using System;
using System.Collections.Generic;
using System.Linq;
using RecipeGame.Inventory;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models 
{
    public class Cauldron 
    {
        public Dictionary<ItemType, InventoryItem> Ingredients { get; set; }

        public float HeatLevel { get; set; }

        public int IngredientVolume {get; set; }

        public float Temperature { get; set; }

        public float CookTimer { get; set; }

        public bool HasAnyLiquids() => Ingredients.Values.Any(i => i.Stats.IsLiquid); 

        public List<InventoryItem> Products { get; set; }

        public BatchedRecipe CurrentRecipe { get; set; }
    }
}