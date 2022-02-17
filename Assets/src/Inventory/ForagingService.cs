using System.Collections.Generic;
using System.Linq;
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
            //TODO look for charm in inventory, use up consumables.

            return RollForItemDrops(biome, forageCount, false).AccumulateItemStacks(InventoryService.StorageItemSlotCount);
        }

        private IEnumerable<InventoryItem> RollForItemDrops(BiomeType biome, int rollCount, bool charmActive) 
        {
            var weightedDropRates = ExpandWeightedDropRates(biome, false).ToArray();

            
            return Enumerable.Range(0, rollCount)
                .Select(i => weightedDropRates[rand.RandiRange(0, weightedDropRates.Length-1)])
                .Select(d => new InventoryItem(){
                    StackAmount = rand.RandiRange(d.probability.StackSizeMin, d.probability.StackSizeMax),
                    Stats = d.stats
                });
        }

        private IEnumerable<(ItemType type, ForageProbability probability, InventoryItemStats stats)> ExpandWeightedDropRates(BiomeType biomeType, bool charmActive)
        {
            var baseDropRates = StatsDefinitions.MappedBiomeStats.Value[biomeType].DropRate;

            foreach(var dropRateKVP in baseDropRates) 
            {
                var weightedCount = dropRateKVP.Value.BaseDropRate * (charmActive ? dropRateKVP.Value.CharmedDropRate : 1);
                for(int i=0; i<weightedCount; i++) 
                {
                    yield return (dropRateKVP.Key, dropRateKVP.Value, StatsDefinitions.MappedItemStats.Value[dropRateKVP.Key]);
                }
            }
        }
    }
}