using IS = RecipeGame.Models.InventoryItemStats;
using BS = RecipeGame.Models.BiomeStats;
using FP = RecipeGame.Models.ForageProbability;
using RS = RecipeGame.Models.RecipeStats;
using RI = RecipeGame.Models.RecipeIngredient;
using RP = RecipeGame.Models.RecipeProduct;
using static RecipeGame.Helpers.Enums;
using System.Collections.Generic;
using System;
using System.Linq;
using Godot;

namespace RecipeGame.Inventory
{
    public static class StatsDefinitions
    {
        public static IS[] ItemStats = new IS[]
        {
            // Forest
            new IS(ItemType.Wood, "Wood", "Wood Chips", 64, 1, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.Grain, "Grain", "Crushed Grain", 64, 2, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.Truffle, "Truffle", "Shaved Truffle", 1, 10, 1, ItemDisplayMode.Single, false),

            // Lake
            new IS(ItemType.FreshWater, "Fresh Water", null, 10000, 1, 1, ItemDisplayMode.Bar, true),
            new IS(ItemType.PeatMoss, "Peat Moss", "Shredded Peat Moss", 64, 2, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.EyeOfNewt, "Eye of Newt", "Jellied Eye of Newt", 64, 2, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.Frog, "Frog", "Diced Frog", 1, 10, 1, ItemDisplayMode.Single, false),

            // Shore
            new IS(ItemType.SaltWater, "Salt Water", null, 10000, 1, 1, ItemDisplayMode.Bar, true),
            new IS(ItemType.Oyster, "Oyster", "Shucked Oyster", 64, 8, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.Jellyfish, "Jellyfish", "Pureed Jellyfish", 1, 40, 1, ItemDisplayMode.Single, false),
            
            // Cave
            new IS(ItemType.CaveSlime, "Cave Slime", null, 10000, 8, 1, ItemDisplayMode.Bar, true),
            new IS(ItemType.Guano, "Guano", "Sifted Guano", 64, 16, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.Lodestone, "Lodestone", "Crushed Lodestone", 1, 80, 1, ItemDisplayMode.Single, false),

            // Recipe
            new IS(ItemType.Gruel, "Gruel", null, 64, 5, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.BogWater, "Bog Water", null, 10000, 40, 1, ItemDisplayMode.Bar, true),
            new IS(ItemType.AmphibianCharm, "Amphibian Charm", "Cubed Amphibian Charm", 1, 300, 1, ItemDisplayMode.Single, false),
            new IS(ItemType.Salt, "Salt", null, 64, 3, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.OysterSoup, "Oyster Soup", null, 64, 80, 1, ItemDisplayMode.Discrete, false),
            new IS(ItemType.GlowCharm, "Glow Charm", "Mashed Glow Charm", 1, 500, 1, ItemDisplayMode.Single, false),
            new IS(ItemType.CharismaCharm, "Charisma Charm", null, 1, 2500, 1, ItemDisplayMode.Single, false),
            new IS(ItemType.BurntMuck, "Burnt Much", "Squished Burnt Much", 64, 1, 1, ItemDisplayMode.Discrete, false),
        };

        public static Lazy<Dictionary<ItemType, IS>> MappedItemStats = new Lazy<Dictionary<ItemType, IS>>(() => {
            var statDict = ItemStats.ToDictionary(s => s.ItemType, s => s);

            foreach(ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                if(!statDict.ContainsKey(itemType))
                {
                    GD.PushWarning($"Item type {Enum.GetName(typeof(ItemType), itemType)} does not have a stats declaration!");
                }
            }

            return statDict;
        });

        public static BS[] BiomeStats = new BS[]
        {
            new BS(BiomeType.Forest, new Dictionary<ItemType, FP>{
                {ItemType.Wood, new FP(100, 2, 10)},
                {ItemType.Grain, new FP(50, 2, 10)},
                {ItemType.Truffle, new FP(5, 1, 1)}
            }),
            new BS(BiomeType.Lake, new Dictionary<ItemType, FP>{
                {ItemType.FreshWater, new FP(100, 800, 1200)},
                {ItemType.PeatMoss, new FP(50, 2, 10)},
                {ItemType.EyeOfNewt, new FP(25, 1, 4)},
                {ItemType.Frog, new FP(5, 1, 1)}
            }),
            new BS(BiomeType.Shore, new Dictionary<ItemType, FP>{
                {ItemType.SaltWater, new FP(100, 800, 1200)},
                {ItemType.Oyster, new FP(25, 1, 4)},
                {ItemType.Jellyfish, new FP(5, 1, 1)}
            }),
            new BS(BiomeType.Cave, new Dictionary<ItemType, FP>{
                {ItemType.CaveSlime, new FP(100, 800, 1200)},
                {ItemType.Guano, new FP(25, 1, 4)},
                {ItemType.Lodestone, new FP(5, 1, 1)}
            })
        };
        
        public static Lazy<Dictionary<BiomeType, BS>> MappedBiomeStats = new Lazy<Dictionary<BiomeType, BS>>(() => BiomeStats.ToDictionary(s => s.BiomeType, s => s));

        public static RS[] RecipeStats = new RS[]
        {
            new RS()
            {
                Name = "Gruel",
                Temperature = new Vector2(100, 1e7f),
                CookTime = 15,
                Ingredients = new RI[]{
                    new RI(ItemType.Grain, new Vector2(4, 4), true),
                    new RI(ItemType.FreshWater, new Vector2(750, 1e7f), false)
                },
                Products = new RP[]{
                    new RP(ItemType.Gruel, 4, 4, 100)
                },
            },
            new RS()
            {
                Name = "Bog Water",
                Temperature = new Vector2(100, 400),
                CookTime = 15,
                Ingredients = new RI[]{
                    new RI(ItemType.PeatMoss, new Vector2(10, 10), true),
                    new RI(ItemType.FreshWater, new Vector2(750, 1e7f), false)
                },
                Products = new RP[]{
                    new RP(ItemType.BogWater, 1000, 1000, 100)
                },

            },
            new RS()
            {
                Name = "Amphibian Charm",
                Temperature = new Vector2(100, 400),
                CookTime = 30,
                Ingredients = new RI[]{
                    new RI(ItemType.Gruel, new Vector2(6, 6), false),
                    new RI(ItemType.BogWater, new Vector2(750, 1e7f), false),
                    new RI(ItemType.EyeOfNewt, new Vector2(10, 20), true),
                    new RI(ItemType.Frog, new Vector2(1, 1), true),
                    new RI(ItemType.Truffle, new Vector2(1, 1), true)
                },
                Products = new RP[]{
                    new RP(ItemType.AmphibianCharm, 1, 1, 100)
                },

            },
            new RS()
            {
                Name = "Salt",
                Temperature = new Vector2(300, 1e7f),
                CookTime = 15,
                Ingredients = new RI[]{
                    new RI(ItemType.SaltWater, new Vector2(800, 1e7f), false),
                },
                Products = new RP[]{
                    new RP(ItemType.Salt, 6, 6, 100)
                },

            },
            new RS()
            {
                Name = "Oyster Soup",
                Temperature = new Vector2(100, 300),
                CookTime = 30,
                Ingredients = new RI[]{
                    new RI(ItemType.FreshWater, new Vector2(750, 1250), false),
                    new RI(ItemType.Salt, new Vector2(2, 2), false),
                    new RI(ItemType.Oyster, new Vector2(4, 8), true),
                    new RI(ItemType.Truffle, new Vector2(1, 1), true)
                },
                Products = new RP[]{
                    new RP(ItemType.OysterSoup, 1, 1, 100)
                },
            },
            new RS()
            {
                Name = "Glow Charm",
                Temperature = new Vector2(100, 400),
                CookTime = 45,
                Ingredients = new RI[]{
                    new RI(ItemType.SaltWater, new Vector2(750, 1250), false),
                    new RI(ItemType.FreshWater, new Vector2(750, 1250), false),
                    new RI(ItemType.Wood, new Vector2(4, 8), true),
                    new RI(ItemType.OysterSoup, new Vector2(4, 4), false),
                    new RI(ItemType.Jellyfish, new Vector2(2, 2), true)
                },
                Products = new RP[]{
                    new RP(ItemType.GlowCharm, 1, 1, 100)
                },
            },
            new RS()
            {
                Name = "Charisma Charm",
                Temperature = new Vector2(200, 400),
                CookTime = 60,
                Ingredients = new RI[]{
                    new RI(ItemType.SaltWater, new Vector2(750, 1250), false),
                    new RI(ItemType.FreshWater, new Vector2(750, 1250), false),
                    new RI(ItemType.BogWater, new Vector2(750, 1250), false),
                    new RI(ItemType.CaveSlime, new Vector2(750, 1250), true),
                    new RI(ItemType.Guano, new Vector2(6, 12), true),
                    new RI(ItemType.Lodestone, new Vector2(1, 1), true),
                    new RI(ItemType.AmphibianCharm, new Vector2(1, 1), true),
                    new RI(ItemType.GlowCharm, new Vector2(1, 1), true),
                },
                Products = new RP[]{
                    new RP(ItemType.CharismaCharm, 1, 1, 100)
                },
            },
            new RS()
            {
                Name = "Burnt Muck",
                Temperature = new Vector2(400, 1e7f),
                CookTime = 10,
                Ingredients = new RI[]{
                    new RI(ItemType.SaltWater, new Vector2(750, 1250), false),
                    new RI(ItemType.FreshWater, new Vector2(750, 1250), false),
                    new RI(ItemType.BogWater, new Vector2(750, 1250), false),
                    new RI(ItemType.CaveSlime, new Vector2(750, 1250), true),
                    new RI(ItemType.Guano, new Vector2(6, 12), true),
                    new RI(ItemType.Lodestone, new Vector2(1, 1), true),
                    new RI(ItemType.AmphibianCharm, new Vector2(1, 1), true),
                    new RI(ItemType.GlowCharm, new Vector2(1, 1), true),
                },
                Products = new RP[]{
                    new RP(ItemType.BurntMuck, 2, 10, 100)
                },
            },
        };
    }
}