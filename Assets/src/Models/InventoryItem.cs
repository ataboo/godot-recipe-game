using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItem
    {
        public int StackAmount { get; set; }

        public InventoryItemStats Stats { get; set; }
    }
}