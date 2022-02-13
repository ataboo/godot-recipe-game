namespace RecipeGame.Models
{
    public class PlayerInventory : IInventory
    {
        public int Money { get; set; }

        public InventoryItem[] Items { get; set; }
    }
}