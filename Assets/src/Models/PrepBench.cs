namespace RecipeGame.Models
{
    public class PrepBench
    {
        public IInventory InputItems { get; set; }

        public IInventory OutputItems { get; set; }

        public InventoryItem InputItem {            
            get => InputItems.Items[0];
            set => InputItems.Items[0] = value;
        }
        
        public InventoryItem OutputItem {            
            get => OutputItems.Items[0];
            set => OutputItems.Items[0] = value;
        }
    }
}