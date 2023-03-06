#nullable enable
using Godot;

namespace Isoland
{
    public static class AutoLoad
    {
        public static T _<T>(this Node node) where T : class
        {
            return node.GetNode<T>($"/root/{typeof(T).Name}");
        }

        public static T _<T>(this Node node, string name) where T : class
        {
            return string.IsNullOrEmpty(name) ? node._<T>() : node.GetNode<T>($"/root/{name}");
        }
    }
}