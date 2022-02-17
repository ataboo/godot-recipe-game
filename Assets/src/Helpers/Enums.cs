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
            Cave,
            Shore
        }

        public enum GameScene 
        {
            Map,
            Cottage,
        }

        public enum HeldTool
        {
            Empty,
            Scoop,
            SlottedSpoon,
            StirSpoon
        }
    }
}