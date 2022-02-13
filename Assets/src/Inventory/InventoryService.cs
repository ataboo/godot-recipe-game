using System.Collections.Generic;
using Godot;
using RecipeGame.Models;

namespace RecipeGame.Inventory
{
    public class InventoryService
    {
        public const int ItemSlotCount = 24;

        public void InitNewInventories(PlayerData data) 
        {
            data.Inventory = new PlayerInventory() 
            {
                Items = new InventoryItem[ItemSlotCount],
                Money = 0,
            };

            data.Cauldron = new Cauldron() 
            {
                Items = new List<InventoryItem>(),
                Temperature = 0f
            };
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

            if(destItem.Stats.TypeID == sourceItem.Stats.TypeID) 
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
    }
}