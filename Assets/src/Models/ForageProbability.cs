namespace RecipeGame.Models
{
    public class ForageProbability
    {
        public int BaseDropRate { get; set; }

        public int CharmedDropRate { get; set; }

        public int StackSizeMin { get; set; }

        public int StackSizeMax { get; set; }

        public ForageProbability(int baseDropRate, int charmedDropRate, int stackSizeMin, int stackSizeMax)
        {
            BaseDropRate = baseDropRate;
            CharmedDropRate = charmedDropRate;
            StackSizeMin = stackSizeMin;
            StackSizeMax = stackSizeMax;
        }
    }
}