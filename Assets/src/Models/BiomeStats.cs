using System.Collections.Generic;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models 
{
    public class BiomeStats 
    {
        public BiomeType BiomeType { get; }

        public IReadOnlyDictionary<ItemType, ForageProbability> DropRate { get; }

        public BiomeStats(BiomeType type, Dictionary<ItemType, ForageProbability> dropRate)
        {
            BiomeType = type;
            DropRate = dropRate;
        }
    }
}