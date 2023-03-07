using Godot;
using GodotUtilities;
using Isoland.items;
using Isoland.objects;

namespace Isoland.globals
{
    public partial class Mailbox : FlagSwitch
    {
        [Node("MailBoxClose/Interactable")]
        private Interactable _interactable;

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

            _interactable.Interact += OnInteractableInteract;
        }

        private void OnInteractableInteract()
        {
            var item = this._<Game>().Inventory.ActiveItem;
            if (item == null || item != GD.Load<Item>("res://items/key.tres"))
            {
                return;
            }
            this._<Game>().Flags.Add(Flag);
            this._<Game>().Inventory.RemoveItem(item);
        }
    }
}
