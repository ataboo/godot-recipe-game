using System.Collections.Generic;
using System.Linq;
using Godot;
using RecipeGame.Models;

namespace RecipeGame.Helpers
{
    public static class InventoryExtensions 
    {
        public static IEnumerable<InventoryItem> AccumulateItemStacks(this IEnumerable<InventoryItem> items)
        {
            var combinedItems = items.OfType<InventoryItem>().GroupBy(i => i.Stats.ItemType).ToDictionary(g => g.Key, g => (g.Sum(i => i.StackAmount), g.First().Stats));
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
                    var itemAmount = Mathf.Min(stats.StackSize, stackRemaining);
                    stackRemaining -= itemAmount;
                    yield return new InventoryItem() 
                    {
                        StackAmount = itemAmount,
                        Stats = stats
                    };
                }
            }
        }

        public static InventoryItem[] AccumulateItemStacks(this IEnumerable<InventoryItem> items, int inventorySize)
        {
            var output = new InventoryItem[inventorySize];
            var idx = 0;
            foreach(var item in AccumulateItemStacks(items))
            {
                output[idx++] = item;

                if(idx >= inventorySize)
                {
                    GD.PushError("Accumulate array overflowed inventory of fixed size.");
                    break;
                }
            }

            return output;
        }
    }
}