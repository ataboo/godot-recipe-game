using System.Collections.Generic;

namespace RecipeGame.Models 
{
    public class Cauldron 
    {
        public List<InventoryItem> Items { get; set; }

        public float Temperature { get; set; }
    }
}