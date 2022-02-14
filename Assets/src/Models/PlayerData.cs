using Godot;

namespace RecipeGame.Models
{
    public class PlayerData
    {
        public PlayerInventory Inventory { get; set; }

        public StorageInventory Storage { get; set; }

        public Cauldron Cauldron { get; set; }

        public PrepBench PrepBench { get; set; }
    }
}