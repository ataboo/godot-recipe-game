namespace RecipeGame.Models
{
    public class ForageProbability
    {
        public int BaseDropRate { get; set; }

        public int StackSizeMin { get; set; }

        public int StackSizeMax { get; set; }

        public ForageProbability(int baseDropRate, int stackSizeMin, int stackSizeMax)
        {
            BaseDropRate = baseDropRate;
            StackSizeMin = stackSizeMin;
            StackSizeMax = stackSizeMax;
        }
    }
}