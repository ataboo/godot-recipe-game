using System.Collections.Generic;
using Godot;
using RecipeGame.Models;
using static RecipeGame.Helpers.Enums;
using static RecipeGame.Helpers.InventoryExtensions;


namespace RecipeGame.Inventory 
{
    public class ForagingService 
    {
        private readonly RandomNumberGenerator rand;

        public ForagingService() 
        {
            rand = new RandomNumberGenerator();
            rand.Randomize();
        }

        public IEnumerable<InventoryItem> ForageForItems(BiomeType biome, int forageCount, PlayerInventory playerInventory) 
        {
            return RollForItemDrops(biome, forageCount, false).AccumulateItemStacks(InventoryService.StorageItemSlotCount);
        }

        private IEnumerable<InventoryItem> RollForItemDrops(BiomeType biome, int rollCount, bool charmActive) 
        {
            var dropRates = StatsDefinitions.MappedBiomeStats.Value[biome].DropRate;

            foreach(var dropKVP in dropRates)
            {
                if(rand.RandiRange(0, 100) <= dropKVP.Value.BaseDropRate)
                {
                    var count = rand.RandiRange(dropKVP.Value.StackSizeMin, dropKVP.Value.StackSizeMax);
                    var stats = StatsDefinitions.MappedItemStats.Value[dropKVP.Key];
                    yield return new InventoryItem
                    {
                        Processed = false,
                        StackAmount = count,
                        Stats = stats
                    };
                }
            }
        }
    }
}