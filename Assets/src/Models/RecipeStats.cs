using Godot;

namespace RecipeGame.Models
{
    public class RecipeStats
    {
        public string Name { get; set; }

        public RecipeIngredient[] Ingredients { get; set; }

        public Vector2 Temperature { get; set; }

        public float CookTime { get; set; }

        public RecipeProduct[] Products { get; set; }
    }
}