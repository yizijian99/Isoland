using Godot;
using Isoland.globals;

namespace Isoland.scenes
{
    public partial class Scene : Sprite2D
    {
        public override void _Ready()
        {
            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(this, "scale", Vector2.One, 0.3)
                .From(Vector2.One * 1.05f);
        }
    }
}