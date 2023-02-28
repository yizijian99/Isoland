using Godot;

namespace Isoland.objects
{
    [GodotClassName(nameof(Interactable))]
    public partial class Interactable : Area2D
    {
        [Signal]
        public delegate void InteractEventHandler();
        
        public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
        {
            if (!@event.IsActionPressed("interact"))
            {
                return;
            }
            _Interact();
        }

        protected virtual void _Interact()
        {
            EmitSignal(SignalName.Interact);
        }
    }
}
