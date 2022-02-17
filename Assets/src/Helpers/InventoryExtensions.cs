using System.Collections.Generic;
using System.Linq;
using Godot;
using RecipeGame.Models;

namespace RecipeGame.Helpers
{
    public static class InventoryExtensions 
    {
        public static InventoryItem[] AccumulateItemStacks(this IEnumerable<InventoryItem> items, int inventorySize)
        {
            var combinedItems = items.OfType<InventoryItem>().GroupBy(i => i.Stats.ItemType).ToDictionary(g => g.Key, g => (g.Sum(i => i.StackAmount), g.First().Stats));
            var outputItems = new InventoryItem[inventorySize];
            var inventoryIdx = 0;

            foreach(var itemKVP in combinedItems) 
            {
                var stackRemaining = itemKVP.Value.Item1;
                var stats = itemKVP.Value.Item2;

                if(stats.StackSize == 0 )
                {
                    throw new System.ArgumentOutOfRangeException("Stack max size must be greater than 0");
                }
                while(stackRemaining > 0) 
                {
                    if(inventoryIdx >= inventorySize) 
                    {
                        throw new System.IndexOutOfRangeException("Inventory size not large enough to accumulate.");
                    }

                    var itemAmount = Mathf.Min(stats.StackSize, stackRemaining);
                    stackRemaining -= itemAmount;
                    outputItems[inventoryIdx++] = new InventoryItem() 
                    {
                        StackAmount = itemAmount,
                        Stats = stats
                    };
                }
            }

            return outputItems.ToArray();
        }
    }
}