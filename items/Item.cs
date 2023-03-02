using Godot;

namespace Isoland.items
{
    [GodotClassName(nameof(Item))]
    public partial class Item : Resource
    {
        [Export] public string Description;
        [Export] public Texture PropTexture;
        [Export] public Texture SceneTexture;

        public static class Const
        {
            public static readonly string Description = nameof(Description);
            public static readonly string PropTexture = nameof(PropTexture);
            public static readonly string SceneTexture = nameof(SceneTexture);
        }
    }
}