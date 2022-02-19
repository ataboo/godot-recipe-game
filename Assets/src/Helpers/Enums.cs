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
            FreshWater,
            SaltWater,
            EyeOfNewt,
            Truffle,
            CaveSlime,
            Oyster,
            Wood,
            PeatMoss,
            BogWater,
            Salt,
            Grain,
            Gruel,
            Frog,
            AmphibianCharm,
            Jellyfish,
            Guano,
            Lodestone,
            OysterSoup,
            GlowCharm,
            CharismaCharm,
            BurntMuck
        }

        public enum BiomeType
        {
            Forest,
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
            StirSpoon,
            Bowl
        }

        public enum CauldronState
        {
            WaitingToCook,
            RecipeInProgress,
            RecipeInterrupted,
            JustFinishedRecipe,
            WaitingForProductRemoval,
            JustStartedRecipe,
        }

        public enum CauldronLiquidColor
        {
            Green,
            Blue,
            Dry
        }
    }
}