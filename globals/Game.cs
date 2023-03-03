using System.Collections.Generic;
using Godot;

namespace Isoland.globals
{
    public partial class Game : Node
    {
        public readonly FlagsClass Flags = new();

        public partial class FlagsClass : Node
        {
            [Signal]
            public delegate void ChangedEventHandler();

            private readonly List<string> _flags = new();

            public bool Has(string str)
            {
                return _flags.Contains(str);
            }

            public void Add(string flag)
            {
                if (_flags.Contains(flag))
                {
                    return;
                }

                _flags.Add(flag);
                EmitSignal(SignalName.Changed);
            }
        }
    }
}