using System;
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

        public const int PrepBenchSlots = 16;
        
        public const int CauldronVolumeCapacity = 40000;

        public void InitNewInventories(PlayerData data) 
        {
            data.Inventory = new PlayerInventory() 
            {
                Items = new InventoryItem[PlayerItemSlotCount],
            };

            data.Storage = new StorageInventory() 
            {
                Items = new InventoryItem[StorageItemSlotCount]
            };

            data.ForageStorage = new StorageInventory()
            {
                Items = new InventoryItem[StorageItemSlotCount]
            };

            data.MarketSell = new StorageInventory()
            {
                Items = new InventoryItem[StorageItemSlotCount]
            };

            data.Cauldron = new Cauldron() 
            {
                Ingredients = new Dictionary<(ItemType type, bool processed), InventoryItem>(),
                Temperature = 0f,
                Products = new List<InventoryItem>(),
                HeatLevel = 0,
                CookTimer = 0,
                CurrentRecipe = null 
            };

            data.PrepBench = new PrepBench
            {
                InputItems = new StorageInventory(){
                    Items = new InventoryItem[1]
                },
                OutputItems = new StorageInventory(){
                    Items = new InventoryItem[1]
                }
            };

            data.HeldTool = HeldTool.Empty;

            data.HeldItem = null;

            data.PurchasedRecipes = StatsDefinitions.RecipeStats.Where(r => r.Price.HasValue).Select(r => r.ItemType).ToDictionary(t => t, t => false);

            // data.Inventory.Items[0] = new InventoryItem
            // {
            //     Processed = false,
            //     StackAmount = 1,
            //     Stats = StatsDefinitions.MappedItemStats.Value[ItemType.GlowCharm]
            // };
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

            if(playerData.HeldTool == HeldTool.Empty)
            {
                if(playerData.HeldItem != null)
                {
                    return PutItemDownInInventory(targetInventory, playerData, index, leftClick);
                }

                return PickItemUpFromInventory(targetInventory, playerData, index, leftClick);
            }

            return MoveItemWithTool(targetInventory, playerData, index, leftClick);
        }

        public bool CanEnterBiome(PlayerData playerData, BiomeType biome)
        {
            switch(biome)
            {
                case BiomeType.Cave:
                    return playerData.Inventory.Items.OfType<InventoryItem>().Any(i => i.Stats.ItemType == ItemType.GlowCharm && !i.Processed);
                case BiomeType.Shore:
                    return playerData.Inventory.Items.OfType<InventoryItem>().Any(i => i.Stats.ItemType == ItemType.AmphibianCharm && !i.Processed);
                default:
                    return true;
            }
        }

        private bool MoveItemWithTool(IInventory targetInventory, PlayerData playerData, int index, bool leftClick)
        {
            InventoryItem targetItem = targetInventory.Items[index];
            var moveAmount = playerData.HeldTool == HeldTool.Scoop ? 1 : 1000;
            if(leftClick)
            {
                if(targetItem == null || targetItem.StackAmount < moveAmount)
                {
                    return false;
                }

                if(playerData.HeldItem == null)
                {
                    playerData.HeldItem = targetItem.Clone(true);
                }
                else
                {
                    if(playerData.HeldItem.Stats.ItemType != targetItem.Stats.ItemType ||
                        playerData.HeldItem.Stats.StackSize - playerData.HeldItem.StackAmount < moveAmount || 
                        playerData.HeldItem.Processed != targetItem.Processed
                    )
                    {
                        return false;
                    }
                }

                targetItem.StackAmount -= moveAmount;
                playerData.HeldItem.StackAmount += moveAmount;
                if(targetItem.StackAmount == 0)
                {
                    targetInventory.Items[index] = null;
                }
            }
            else
            {
                if(playerData.HeldItem == null || playerData.HeldItem.StackAmount < moveAmount)
                {
                    return false;
                }

                if(targetItem == null)
                {
                    targetItem = playerData.HeldItem.Clone(true);
                    targetInventory.Items[index] = targetItem;
                }
                else
                {
                    if(
                        targetItem.Stats.ItemType != playerData.HeldItem.Stats.ItemType || 
                        targetItem.Stats.StackSize - targetItem.StackAmount < moveAmount ||
                        targetItem.Processed != playerData.HeldItem.Processed
                    )
                    {
                        return false;
                    }
                }

                playerData.HeldItem.StackAmount -= moveAmount;
                targetItem.StackAmount += moveAmount;
                if(playerData.HeldItem.StackAmount == 0)
                {
                    playerData.HeldItem = null;
                }
            }

            return true;
        }

        private bool PutItemDownInInventory(IInventory targetInventory, PlayerData playerData, int index, bool leftClick)
        {
            if(playerData.HeldItem == null)
            {
                return false;
            }

            var targetItem = targetInventory.Items[index];
            var dropAmount = leftClick ? playerData.HeldItem.StackAmount : playerData.HeldItem.StackAmount / 2;

            if(targetItem == null)
            {
                targetItem = playerData.HeldItem.Clone(true);
                targetInventory.Items[index] = targetItem;
            }
            else
            {
                if(targetItem.Stats.ItemType != playerData.HeldItem.Stats.ItemType || targetItem.Processed != playerData.HeldItem.Processed)
                {
                    return false;
                }

                dropAmount = Mathf.Min(targetItem.Stats.StackSize - targetItem.StackAmount, dropAmount);
            }

            playerData.HeldItem.StackAmount -= dropAmount;
            if(playerData.HeldItem.StackAmount == 0)
            {
                playerData.HeldItem = null;
            }
            targetItem.StackAmount += dropAmount;

            return dropAmount > 0;
        }

        private bool PickItemUpFromInventory(IInventory targetInventory, PlayerData playerData, int index, bool leftClick)
        {
            var targetItem = targetInventory.Items[index];
            if(targetItem == null)
            {
                return false;
            }

            var pickupAmount = leftClick ? targetItem.StackAmount : targetItem.StackAmount / 2;

            if(playerData.HeldItem == null)
            {
                playerData.HeldItem = targetItem.Clone(true);
            }
            else 
            {
                if(targetItem.Stats.ItemType != playerData.HeldItem.Stats.ItemType || targetItem.Processed != playerData.HeldItem.Processed)
                {
                    return false;
                }
                pickupAmount = Mathf.Min(pickupAmount, playerData.HeldItem.Stats.StackSize - playerData.HeldItem.StackAmount);
            }

            targetItem.StackAmount -= pickupAmount;
            if(targetItem.StackAmount == 0)
            {
                targetInventory.Items[index] = null;
            }
            playerData.HeldItem.StackAmount += pickupAmount;

            return pickupAmount > 0;
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

            var matchingDestItems = destInventory.Items
                .OfType<InventoryItem>()
                .Where(i => i.Stats.ItemType == sourceItem.Stats.ItemType && i.Processed == sourceItem.Processed);

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

        public bool BuyRecipe(PlayerData playerData, ItemType itemType)
        {
            if(!playerData.PurchasedRecipes.ContainsKey(itemType))
            {
                GD.PushError($"Failed to purchase recipe: {itemType}");
                return false;
            }

            var price = StatsDefinitions.MappedRecipeStats.Value[itemType].Price;
            if(!price.HasValue || playerData.CoinBalance < price.Value)
            {
                return false;
            }

            playerData.CoinBalance -= price.Value;
            playerData.PurchasedRecipes[itemType] = true;

            return true;
        }

        public void SellMarketItems(PlayerData playerData)
        {
            var value = GetSellMarketValue(playerData);
            playerData.MarketSell.Items = new InventoryItem[InventoryService.StorageItemSlotCount];
            playerData.CoinBalance += value;
        }

        public int GetSellMarketValue(PlayerData playerData)
        {
            return playerData.MarketSell.Items.OfType<InventoryItem>().Sum(i => i.StackAmount * i.Stats.BasePrice / (i.Stats.IsLiquid ? 1000 : 1));
        }

        public bool ProcessPrepBench(PrepBench prepBench)
        {
            if(!PrepBenchCanProcess(prepBench))
            {
                return false;
            }

            if(prepBench.OutputItem == null)
            {
                prepBench.OutputItem = prepBench.InputItem.Clone(true);
                prepBench.OutputItem.Processed = true;
            }

            prepBench.InputItem.StackAmount -= 1;
            prepBench.OutputItem.StackAmount += 1;

            if(prepBench.InputItem.StackAmount == 0)
            {
                prepBench.InputItem = null;
            }

            return true;
        }

        public bool PrepBenchCanProcess(PrepBench prepBench)
        {
            if(prepBench.InputItem == null || string.IsNullOrEmpty(prepBench.InputItem.Stats.ProcessedName) || prepBench.InputItem.Stats.IsLiquid)
            {
                return false;
            }

            if(prepBench.OutputItem == null)
            {
                return true;
            }

            if(prepBench.OutputItem.Stats.ItemType != prepBench.InputItem.Stats.ItemType || !prepBench.OutputItem.Processed)
            {
                return false;
            }

            if(prepBench.OutputItem.StackAmount == prepBench.OutputItem.Stats.StackSize)
            {
                return false;
            }

            return true;
        }

        public bool MoveStack(IInventory sourceInventory, IInventory destInventory, int sourceIdx) 
        {
            if(sourceInventory == null || destInventory == null)
            {
                return false;
            }

            var movedPartialStack = false;
            var sourceItem = sourceInventory.Items[sourceIdx];
            if(sourceItem == null) 
            {
                GD.PushError("Failed to move a null item.");
                return false;
            }

            var matchingDestItems = destInventory.Items
                .OfType<InventoryItem>()
                .Where(i => i.Stats.ItemType == sourceItem.Stats.ItemType && i.Processed == sourceItem.Processed);

            foreach(var destItem in matchingDestItems)
            {
                var transferAmount = Mathf.Min(destItem.Stats.StackSize - destItem.StackAmount, sourceItem.StackAmount);
                if(transferAmount == 0)
                {
                    continue;
                }

                movedPartialStack = true;
                sourceItem.StackAmount -= transferAmount;
                destItem.StackAmount += transferAmount;

                if(sourceItem.StackAmount == 0) 
                {
                    sourceInventory.Items[sourceIdx] = null;
                    return true;
                }
            }

            for(int i=0; i<destInventory.Items.Length; i++) 
            {
                if(destInventory.Items[i] == null) 
                {
                    sourceInventory.Items[sourceIdx] = null;
                    destInventory.Items[i] = sourceItem;
                    return true;
                }
            }

            return movedPartialStack;
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
            var remainingCapacity = CauldronVolumeCapacity - cauldron.IngredientVolume;
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

            if(cauldron.Ingredients.TryGetValue((item.Stats.ItemType, item.Processed), out var mergeItem)) 
            {
                // Item type already exists in cauldron, add to stack.
                mergeItem.StackAmount += amount;
            } 
            else 
            {
                // New type in cauldron.
                cauldron.Ingredients[(item.Stats.ItemType, item.Processed)] = item.Clone();
                cauldron.Ingredients[(item.Stats.ItemType, item.Processed)].StackAmount = amount;
            }
        }

        public void DumpCauldron(Cauldron cauldron) 
        {
            cauldron.Ingredients.Clear();
        }

        public void RemoveProductFromCauldron()
        {
            throw new System.NotImplementedException();
        }
    }
}