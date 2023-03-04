using Godot;
using Isoland.mini_game;
using Isoland.objects;

namespace Isoland.scenes
{
    public partial class H2A : Scene
    {
        private Board _board;

        private Interactable _reset;
        
        private Sprite2D _gear;

        public override void _Ready()
        {
            base._Ready();
            _board = GetNode<Board>("Board");
            _reset = GetNode<Interactable>("Reset");
            _gear = GetNode<Sprite2D>("Reset/Sprite2D");

            _reset.Connect(Interactable.SignalName.Interact, Callable.From(OnResetInteract));
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