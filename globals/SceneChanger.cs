using Godot;
using Isoland.scenes;

namespace Isoland.globals
{
    public partial class SceneChanger : CanvasLayer
    {
        private ColorRect _colorRect;

        [Signal]
        public delegate void GameEnteredEventHandler();

        [Signal]
        public delegate void GameExitedEventHandler();

        public override void _Ready()
        {
            _colorRect = GetNode<ColorRect>("ColorRect");
        }

        public void ChangeScene(string path)
        {
            var tween = CreateTween();
            tween.TweenCallback(Callable.From(() => _colorRect.Show()));
            tween.TweenProperty(_colorRect, "color:a", 1.0f, 0.2);
            tween.TweenCallback(Callable.From(() => ChangeSceneToFile(path)));
            tween.TweenProperty(_colorRect, "color:a", 0.0f, 0.3);
            tween.TweenCallback(Callable.From(() => _colorRect.Hide()));
        }

        private void ChangeSceneToFile(string path)
        {
            var oldScene = GetTree().CurrentScene;
            var newScene = GD.Load<PackedScene>(path).Instantiate();

            var root = GetTree().Root;
            root.RemoveChild(oldScene);
            root.AddChild(newScene);
            GetTree().CurrentScene = newScene;

            var wasInGame = oldScene is Scene;
            var isInGame = newScene is Scene;
            if (wasInGame != isInGame)
            {
                if (isInGame)
                {
                    EmitSignal(SignalName.GameEntered);
                }
                else
                {
                    EmitSignal(SignalName.GameExited);
                }
            }
            
            oldScene.QueueFree();
        }
    }
}