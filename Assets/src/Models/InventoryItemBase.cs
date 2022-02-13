using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItemStats
    {
        public int TypeID { get; }

        public string Name { get; } = "Ingredient";

        public string Description { get; } = "This is the description.";

        public int StackSize { get; } = 1;

        public float BasePrice { get; }

        public ItemDisplayMode DisplayMode { get; } = ItemDisplayMode.Single;

        public InventoryItemStats(int typeID, string name, string desciption, int stackSize, float basePrice, ItemDisplayMode displayMode) 
        {
            TypeID = typeID;
            Name = name;
            Description = desciption;
            StackSize = stackSize;
            BasePrice = basePrice;
            DisplayMode = displayMode;
        }
    }
}