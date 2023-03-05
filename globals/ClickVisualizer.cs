using Godot;

namespace Isoland.globals
{
    public partial class ClickVisualizer : CanvasLayer
    {
        public override void _Ready()
        {
            base._Ready();

            Layer = 99;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (!@event.IsActionPressed("interact"))
            {
                return;
            }

            var sprite2D = new Sprite2D
            {
                Texture = GD.Load<Texture2D>("res://assets/UI/click.svg"),
                GlobalPosition = GetViewport().GetMousePosition()
            };
            AddChild(sprite2D);

            var tween = CreateTween();
            tween.TweenProperty(sprite2D, "scale", Vector2.One, 0.3).From(Vector2.One * 0.9f);
            tween.Parallel().TweenProperty(sprite2D, "modulate:a", 1.0f, 0.2).From(0.0f);
            tween.TweenProperty(sprite2D, "modulate:a", 0.0f, 0.3);
            tween.TweenCallback(Callable.From(() => sprite2D.QueueFree()));
        }
    }
}