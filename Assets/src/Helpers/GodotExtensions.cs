using System.Collections.Generic;

namespace RecipeGame.Helpers
{
    public static class GodotExtensions
    {
        public static IEnumerable<TItem> AsEnumerable<TItem>(this Godot.Collections.Array array) where TItem : class
        {
            foreach (var item in array)
            {
                yield return (TItem)item;
            }
        }
    }
}