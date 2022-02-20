using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItemStats
    {
        public ItemType ItemType { get; }

        public string Name { get; }

        public string ProcessedName { get; }

        public int StackSize { get; }

        public int BasePrice { get; }

        public int UnitVolume { get; }

        public ItemDisplayMode DisplayMode { get; }

        public bool IsLiquid { get; set; }

        public InventoryItemStats(ItemType type, string name, string desciption, int stackSize, int basePrice, int unitVolume, ItemDisplayMode displayMode, bool isLiquid) 
        {
            
            ItemType = type;
            Name = name;
            ProcessedName = desciption;
            StackSize = stackSize;
            BasePrice = basePrice;
            UnitVolume = unitVolume;
            DisplayMode = displayMode;
            IsLiquid = isLiquid;
        }
    }
}