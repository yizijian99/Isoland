using Godot;
using GodotUtilities;
using Isoland.scenes;

namespace Isoland.globals
{
    public partial class SceneChanger : CanvasLayer
    {
        [Node("ColorRect")]
        private ColorRect _colorRect;

        [Export(PropertyHint.File, "*.mp3")] public string DefaultMusic;

        [Signal]
        public delegate void GameEnteredEventHandler();

        [Signal]
        public delegate void GameExitedEventHandler();

        public override void _Notification(int what)
        {
            base._Notification(what);
            if (what == NotificationSceneInstantiated)
            {
                this.WireNodes();
            }
        }

        public override void _Ready()
        {
            
            var soundManager = GetNode<SoundManager>($"/root/{nameof(SoundManager)}");
            soundManager.PlayMusic(DefaultMusic);
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
            OnSceneChanged(oldScene, newScene);
            oldScene.QueueFree();
        }

        private void OnSceneChanged(Node oldScene, Node newScene)
        {
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

            var music = DefaultMusic;
            var musicOverride = (string) newScene.Get(nameof(Scene.PropertyName.MusicOverride));
            if (isInGame && !string.IsNullOrEmpty(musicOverride))
            {
                music = musicOverride;
            }

            var soundManager = GetNode<SoundManager>($"/root/{nameof(SoundManager)}");
            soundManager.PlayMusic(music);
        }
    }
}