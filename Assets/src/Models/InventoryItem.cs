using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItem
    {
        public int StackAmount { get; set; }

        public bool Processed { get; set; }
        
        public InventoryItemStats Stats { get; set; }

        public int Volume => StackAmount * Stats.UnitVolume;

        public string DisplayName => Processed ? Stats.ProcessedName : Stats.Name;

        public InventoryItem Clone(bool empty = false) 
        {
            return new InventoryItem 
            {
                StackAmount = empty ? 0 : this.StackAmount,
                Stats = this.Stats,
                Processed = this.Processed
            };
        }
    }
}