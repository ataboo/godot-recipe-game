using System.Collections.Generic;
using Godot;

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

        public static TNode MustGetNode<TNode>(this Node node, string path) where TNode : Node
        {
            return node.GetNode<TNode>(path) ?? throw new System.NullReferenceException();
        }

        public static TNode MustGetNode<TNode>(this Node node, NodePath path) where TNode : Node
        {
            return node.GetNode<TNode>(path) ?? throw new System.NullReferenceException();
        }
    }
}