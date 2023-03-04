using Godot;
using Isoland.globals;

namespace Isoland.objects
{
    public partial class FlagSwitch : Node2D
    {
        private Game _game;
        
        [Export] public string Flag;

        private Node2D _defaultNode;    // Flag不存在时，默认显示的节点

        private Node2D _switchNode;     // Flag存在时，切换显示的节点

        public override void _Ready()
        {
            base._Ready();
            _game = GetNode<Game>($"/root/{nameof(Game)}");
            
            var count = GetChildCount();
            if (count > 0)
            {
                _defaultNode = GetChild(0) as Node2D;
            }

            if (count > 1)
            {
                _switchNode = GetChild(1) as Node2D;
            }

            _game.Flags.Connect(Game.SignalName.Changed, Callable.From(UpdateNodes));
            UpdateNodes();
        }

        private void UpdateNodes()
        {
            var exists = _game.Flags.Has(Flag);
            if (_defaultNode != null)
            {
                _defaultNode.Visible = !exists;
            }

            if (_switchNode != null)
            {
                _switchNode.Visible = exists;
            }
        }
    }
}
