using Godot;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class PlayerData
    {
        public PlayerInventory Inventory { get; set; }

        public StorageInventory Storage { get; set; }

        public StorageInventory ForageStorage { get; set; }

        public InventoryItem HeldItem { get; set; }

        public Cauldron Cauldron { get; set; }

        public PrepBench PrepBench { get; set; }

        public HeldTool HeldTool { get; set; }
    }
}