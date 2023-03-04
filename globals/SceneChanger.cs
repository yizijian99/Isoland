using Godot;

namespace Isoland.globals
{
    public partial class SceneChanger : CanvasLayer
    {
        private ColorRect _colorRect;

        public override void _Ready()
        {
            _colorRect = GetNode<ColorRect>("ColorRect");
        }

        public void ChangeScene(string path)
        {
            var tween = CreateTween();
            tween.TweenCallback(Callable.From(() => _colorRect.Show()));
            tween.TweenProperty(_colorRect, "color:a", 1.0f, 0.2);
            tween.TweenCallback(Callable.From(() => GetTree().ChangeSceneToFile(path)));
            tween.TweenProperty(_colorRect, "color:a", 0.0f, 0.3);
            tween.TweenCallback(Callable.From(() => _colorRect.Hide()));
        }
    }
}