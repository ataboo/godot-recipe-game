using Godot;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class RecipeStats
    {
        public ItemType ItemType { get; set; }

        public int? Price { get; set; }

        public RecipeIngredient[] Ingredients { get; set; }

        public Vector2 Temperature { get; set; }

        public float CookTime { get; set; }

        public RecipeProduct[] Products { get; set; }
    }
}