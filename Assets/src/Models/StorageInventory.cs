namespace RecipeGame.Models
{
    public class StorageInventory : IInventory
    {
        public InventoryItem[] Items { get; set; }
    }
}