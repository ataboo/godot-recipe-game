using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItemStats
    {
        public ItemType ItemType { get; }

        public string Name { get; } = "Ingredient";

        public string Description { get; } = "This is the description.";

        public int StackSize { get; } = 1;

        public float BasePrice { get; }

        public int UnitVolume { get; }

        public bool Evaporates { get; }

        public ItemDisplayMode DisplayMode { get; } = ItemDisplayMode.Single;

        public InventoryItemStats(ItemType type, string name, string desciption, int stackSize, float basePrice, int unitVolume, bool evaporates, ItemDisplayMode displayMode) 
        {
            
            ItemType = type;
            Name = name;
            Description = desciption;
            StackSize = stackSize;
            BasePrice = basePrice;
            UnitVolume = unitVolume;
            Evaporates = evaporates;
            DisplayMode = displayMode;
        }
    }
}