namespace RecipeGame.Models
{
    public class PlayerInventory : IInventory
    {
        public InventoryItem[] Items { get; set; }
    }
}