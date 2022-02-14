using IS = RecipeGame.Models.InventoryItemStats;
using BS = RecipeGame.Models.BiomeStats;
using FP = RecipeGame.Models.ForageProbability;
using static RecipeGame.Helpers.Enums;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RecipeGame.Inventory
{
    public static class StatsDefinitions
    {
        public static IS[] ItemStats = new IS[]
        {
            new IS(ItemType.LakeWater, "Lake Water", "Comes from the lake.", 40000, 1f, 1, true, ItemDisplayMode.Bar),
            new IS(ItemType.BogWater, "Bog Water", "Comes from the bog.", 40000, 1f, 1, true, ItemDisplayMode.Bar),
            
            new IS(ItemType.EyeOfNewt, "Eye of Newt", "", 64, 1f, 1, false, ItemDisplayMode.Discrete),
            new IS(ItemType.Toadstool, "Toadstool", "", 64, 1f, 1, false, ItemDisplayMode.Discrete),
            new IS(ItemType.Slime, "Cave Slime", "", 64, 1f, 1, false, ItemDisplayMode.Discrete)
        };

        public static Lazy<Dictionary<ItemType, IS>> MappedItemStats = new Lazy<Dictionary<ItemType, IS>>(() => ItemStats.ToDictionary(s => s.ItemType, s => s));

        public static BS[] BiomeStats = new BS[]
        {
            new BS(BiomeType.Cave, new Dictionary<ItemType, FP>{
                {ItemType.Slime, new FP(1, 1, 1, 4)}
            }),
            new BS(BiomeType.Forest, new Dictionary<ItemType, FP>{
                {ItemType.Toadstool, new FP(1, 1, 1, 4)}
            }),
            new BS(BiomeType.Lake, new Dictionary<ItemType, FP>{
                {ItemType.LakeWater, new FP(1, 1, 1000, 4000)},
                {ItemType.EyeOfNewt, new FP(1, 1, 1, 4)}
            }),
            new BS(BiomeType.Swamp, new Dictionary<ItemType, FP>{
                {ItemType.BogWater, new FP(1, 1, 1000, 4000)}
            }),
        };
        
        public static Lazy<Dictionary<BiomeType, BS>> MappedBiomeStats = new Lazy<Dictionary<BiomeType, BS>>(() => BiomeStats.ToDictionary(s => s.BiomeType, s => s));
    }
}