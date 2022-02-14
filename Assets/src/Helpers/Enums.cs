namespace RecipeGame.Helpers 
{
    public static class Enums 
    {
        public enum ItemDisplayMode 
        {
            Discrete,
            Bar,
            Single
        }

        public enum ItemType 
        {
            LakeWater,
            BogWater,

            EyeOfNewt,
            Toadstool,
            Slime
        }

        public enum BiomeType
        {
            Forest,
            Swamp,
            Lake,
            Cave
        }

        public enum GameScene 
        {
            Map,
            Cottage,
        }
    }
}