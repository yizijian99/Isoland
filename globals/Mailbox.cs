using Godot;
using Isoland.items;
using Isoland.objects;

namespace Isoland.globals
{
    public partial class Mailbox : FlagSwitch
    {
        private Game _game;

        private Interactable _interactable;

        public override void _Ready()
        {
            base._Ready();
            _game = GetNode<Game>($"/root/{nameof(Game)}");
            _interactable = GetNode<Interactable>("MailBoxClose/Interactable");

            _interactable.Connect(Interactable.SignalName.Interact, Callable.From(OnInteractableInteract));
        }

        private void OnInteractableInteract()
        {
            var item = _game.Inventory.ActiveItem;
            if (item == null || item != GD.Load<Item>("res://items/key.tres"))
            {
                return;
            }
            _game.Flags.Add(Flag);
            _game.Inventory.RemoveItem(item);
        }
    }
}
