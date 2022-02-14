using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class InventoryItem
    {
        public int StackAmount { get; set; }

        public InventoryItemStats Stats { get; set; }

        public int Volume => StackAmount * Stats.UnitVolume;
    
        public InventoryItem Clone() 
        {
            return new InventoryItem 
            {
                StackAmount = this.StackAmount,
                Stats = this.Stats
            };
        }
    }
}