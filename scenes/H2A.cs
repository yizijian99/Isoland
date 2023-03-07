using Godot;
using GodotUtilities;
using Isoland.mini_game;
using Isoland.objects;

namespace Isoland.scenes
{
    public partial class H2A : Scene
    {
        [Node("Board")]
        private Board _board;

        [Node("Reset")]
        private Interactable _reset;
        
        [Node("Reset/Sprite2D")]
        private Sprite2D _gear;

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
            base._Ready();
            _reset.Interact += OnResetInteract;
        }

        private void OnResetInteract()
        {
            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(_gear, "rotation_degrees", 360.0f, 0.2).AsRelative();
            tween.TweenCallback(Callable.From(() => _board.Reset()));
        }
    }
}