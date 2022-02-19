using Godot;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Models
{
    public class RecipeIngredient
    {   
        public Vector2 Range { get; set; }

        public ItemType ItemType {get; set;}

        public bool Processed { get; set; }

        public RecipeIngredient(ItemType type, Vector2 Range, bool processed)
        {
            ItemType = type;
            this.Range = Range;
            Processed = processed;
        }
    }
}