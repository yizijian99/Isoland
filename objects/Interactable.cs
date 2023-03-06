using Godot;
using Isoland.globals;

namespace Isoland.objects
{
    [Tool]
    public partial class Interactable : Area2D
    {
        [Signal]
        public delegate void InteractEventHandler();

        [Export] public bool AllowItem;
        
        private Texture2D _textureVariable;

        [Export]
        public Texture2D TextureVariable
        {
            get => _textureVariable;
            set
            {
                _textureVariable = value;

                foreach (var node in GetChildren())
                {
                    if (node.Owner == null)
                    {
                        node.QueueFree();
                    }
                }

                if (_textureVariable == null)
                {
                    return;
                }

                var sprite2D = new Sprite2D {Texture = _textureVariable};
                AddChild(sprite2D);

                var rect = new RectangleShape2D {Size = value.GetSize()};

                var collider = new CollisionShape2D {Shape = rect};
                AddChild(collider);
            }
        }

        public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
        {
            if (!@event.IsActionPressed("interact"))
            {
                return;
            }

            if (this._<Game>().Inventory.ActiveItem != null && !AllowItem)
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