using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class RecipeProduct
    {
        public ItemType ItemType {get; set;}

        public int MaxCount { get; set; }

        public int MinCount { get; set; }

        public int DropProbability { get; set; }

        public RecipeProduct(ItemType type, int maxCount, int minCount, int dropProbability)
        {
            ItemType = type;
            MaxCount = maxCount;
            MinCount = minCount;
            DropProbability = dropProbability;
        }
    }
}