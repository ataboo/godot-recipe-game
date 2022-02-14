using System.Collections.Generic;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models 
{
    public class Cauldron 
    {
        public Dictionary<ItemType, InventoryItem> Items { get; set; }

        public float Temperature { get; set; }

        public int VolumeCapacity { get; set; }
    }
}