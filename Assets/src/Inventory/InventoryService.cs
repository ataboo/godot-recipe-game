using System.Collections.Generic;
using System.Linq;
using Godot;
using RecipeGame.Models;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Inventory
{
    public class InventoryService
    {
        public const int PlayerItemSlotCount = 32;
        public const int StorageItemSlotCount = 72;
        
        public const int CauldronVolumeCapacity = 40000;

        public void InitNewInventories(PlayerData data) 
        {
            data.Inventory = new PlayerInventory() 
            {
                Items = new InventoryItem[PlayerItemSlotCount],
                Money = 0,
            };

            data.Storage = new StorageInventory() 
            {
                Items = new InventoryItem[StorageItemSlotCount]
            };

            data.ForageStorage = new StorageInventory()
            {
                Items = new InventoryItem[StorageItemSlotCount]
            };

            data.Cauldron = new Cauldron() 
            {
                Items = new Dictionary<ItemType, InventoryItem>(),
                Temperature = 0f,
                VolumeCapacity = CauldronVolumeCapacity
            };

            data.PrepBench = new PrepBench
            {
                CurrentItem = null
            };

            data.HeldTool = HeldTool.Empty;

            data.HeldItem = null;
        }

        public void MoveItem(IInventory sourceInventory, IInventory destInventory, int sourceIdx, int destIdx, int amount) 
        {
            var sourceItem = sourceInventory.Items[sourceIdx];
            var destItem = destInventory.Items[destIdx];

            if(sourceItem == null) 
            {
                GD.PushError("Failed to move a null item.");
                return;
            }

            amount = Mathf.Min(amount, sourceItem.StackAmount);

            if(destItem == null) 
            {
                if(amount == sourceItem.StackAmount) 
                {
                    // Move item to empty spot.
                    destInventory.Items[destIdx] = sourceItem;
                    sourceInventory.Items[sourceIdx] = null;
                    
                    return;
                }

                // Split into new stack.
                sourceItem.StackAmount -= amount;
                destInventory.Items[destIdx] = new InventoryItem 
                {
                    StackAmount = amount,
                    Stats = sourceItem.Stats
                };

                return;
            }

            if(destItem.Stats.ItemType == sourceItem.Stats.ItemType) 
            {
                // Combine into dest stack.
                var destCapacity = destItem.Stats.StackSize - destItem.StackAmount;
                amount = Mathf.Min(destCapacity, amount);

                if(amount == sourceItem.StackAmount) 
                {
                    sourceInventory.Items[sourceIdx] = null;
                }

                destItem.StackAmount += amount;
                return;                    
            }

            // Swap stacks when different type.
            destInventory.Items[destIdx] = sourceItem;
            sourceInventory.Items[sourceIdx] = destItem;
        }

        public bool ClickedOnInventoryItem(IInventory targetInventory, PlayerData playerData, int index, bool leftClick)
        {
            InventoryItem targetItem = targetInventory.Items[index];

            if(targetItem == null && playerData.HeldItem == null)
            {
                //No change since both are null.
                return false;
            }

            if(targetItem != null && playerData.HeldItem != null)
            {
                if(targetItem.Stats.ItemType != playerData.HeldItem.Stats.ItemType)
                {
                    // Ignoring since items in both that don't match type.
                    return false;
                }

                var amount = leftClick ? playerData.HeldItem.StackAmount : playerData.HeldItem.StackAmount / 2;

                amount = Mathf.Min(targetItem.Stats.StackSize - targetItem.StackAmount, amount);
                if(playerData.HeldItem.StackAmount > amount)
                {
                    playerData.HeldItem.StackAmount -= amount;
                } 
                else
                {
                    playerData.HeldItem = null;
                }

                targetItem.StackAmount += amount;

                // Attempted to move max amount of whatever was in hand (or half on right click) to stack.
                return amount > 0;
            }

            if(leftClick)
            {
                targetItem = playerData.HeldItem;
                playerData.HeldItem = targetInventory.Items[index];
                targetInventory.Items[index] = targetItem;

                // Swap full item stacks
                return true;
            }

            if(playerData.HeldItem == null)
            {
                if(targetItem.StackAmount <= 1)
                {
                    return false;
                }

                playerData.HeldItem = targetInventory.Items[index].Clone();
                playerData.HeldItem.StackAmount /= 2;
                targetInventory.Items[index].StackAmount -= playerData.HeldItem.StackAmount;
                
                // Take half of inventory stack in hand.
                return true;
            }

            if(playerData.HeldItem.StackAmount <= 1)
            {
                return false;
            }
            targetInventory.Items[index] = playerData.HeldItem.Clone();
            targetInventory.Items[index].StackAmount /= 2;
            playerData.HeldItem.StackAmount -= targetInventory.Items[index].StackAmount;

            // Put half of hand in inventory.
            return true;
        }

        public void EmptyPlayerHand(PlayerData playerData)
        {
            playerData.HeldTool = HeldTool.Empty;
            MoveItemOrTrashIt(playerData.HeldItem, playerData.Inventory);
            playerData.HeldItem = null;
        }

        public void MoveItemOrTrashIt(InventoryItem sourceItem, IInventory destInventory)
        {
            if(sourceItem == null) 
            {
                return;
            }

            var matchingDestItems = destInventory.Items.OfType<InventoryItem>().Where(i => i.Stats.ItemType == sourceItem.Stats.ItemType);

            foreach(var destItem in matchingDestItems)
            {
                var transferAmount = Mathf.Min(destItem.Stats.StackSize - destItem.StackAmount, sourceItem.StackAmount);
                sourceItem.StackAmount -= transferAmount;
                destItem.StackAmount += transferAmount;

                if(sourceItem.StackAmount == 0) 
                {
                    return;
                }
            }

            for(int i=0; i<destInventory.Items.Length; i++) 
            {
                if(destInventory.Items[i] == null) 
                {
                    destInventory.Items[i] = sourceItem;
                    return;
                }
            }
        }

        public void MoveStack(IInventory sourceInventory, IInventory destInventory, int sourceIdx) 
        {
            var sourceItem = sourceInventory.Items[sourceIdx];
            if(sourceItem == null) 
            {
                GD.PushError("Failed to move a null item.");
                return;
            }

            var matchingDestItems = destInventory.Items.Where(i => i.Stats.ItemType == sourceItem.Stats.ItemType);

            foreach(var destItem in matchingDestItems)
            {
                var transferAmount = Mathf.Min(destItem.Stats.StackSize - destItem.StackAmount, sourceItem.StackAmount);
                sourceItem.StackAmount -= transferAmount;
                destItem.StackAmount += transferAmount;

                if(sourceItem.StackAmount == 0) 
                {
                    sourceInventory.Items[sourceIdx] = null;
                    return;
                }
            }

            for(int i=0; i<destInventory.Items.Length; i++) 
            {
                if(destInventory.Items[i] == null) 
                {
                    sourceInventory.Items[sourceIdx] = null;
                    destInventory.Items[i] = sourceItem;
                    return;
                }
            }
        }

        public void AddToCauldron(IInventory inventory, int itemIdx, int amount, Cauldron cauldron) 
        {
            var item = inventory.Items[itemIdx];
            if(item == null) 
            {
                GD.PushError("Failed to move a null item.");
                return;
            }

            amount = Mathf.Min(item.StackAmount, amount);

            var incomingVolume = item.Stats.UnitVolume * amount;
            var remainingCapacity = cauldron.VolumeCapacity - cauldron.Items.Values.Sum(i => i.Volume);
            if(remainingCapacity < incomingVolume) 
            {
                // Reduce amount so it will fit in cauldron
                amount = remainingCapacity / item.Stats.UnitVolume;
            }

            if(amount == item.StackAmount) 
            {
                // Amount is full stack, remove from inventory.
                inventory.Items[itemIdx] = null;
            }
            else 
            {
                // Amount is partial stack, remove part of stack.
                item.StackAmount -= amount;
            }

            if(cauldron.Items.TryGetValue(item.Stats.ItemType, out var mergeItem)) 
            {
                // Item type already exists in cauldron, add to stack.
                mergeItem.StackAmount += amount;
            } 
            else 
            {
                // New type in cauldron.
                cauldron.Items[item.Stats.ItemType] = item.Clone();
                cauldron.Items[item.Stats.ItemType].StackAmount = amount;
            }
        }

        public void DumpCauldron(Cauldron cauldron) 
        {
            cauldron.Items.Clear();
        }

        public void RemoveProductFromCauldron()
        {
            throw new System.NotImplementedException();
        }
    }
}