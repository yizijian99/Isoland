using System;
using System.Collections.Generic;
using Godot;
using Isoland.items;

namespace Isoland.globals
{
    public partial class Game : Node
    {
        [Signal]
        public delegate void ChangedEventHandler();

        public readonly FlagsClass Flags = new();
        public readonly InventoryClass Inventory = new();
        
        public void BackToTitle()
        {
            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.ChangeScene("res://ui/TitleScreen.tscn");
        }
    }

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

    public partial class InventoryClass : Node
    {
        [Signal]
        public delegate void ChangedEventHandler();

        public Item ActiveItem;

        private readonly List<Item> _items = new();
        private int _currentItemIndex = -1;

        public int GetItemCount()
        {
            return _items.Count;
        }

        public Item GetCurrentItem()
        {
            return _currentItemIndex < 0 ? null : _items[_currentItemIndex];
        }

        public void AddItem(Item item)
        {
            if (item == null)
            {
                return;
            }

            if (_items.Contains(item))
            {
                return;
            }

            _items.Add(item);
            _currentItemIndex = _items.Count - 1;
            EmitSignal(SignalName.Changed);
        }

        public void RemoveItem(Item item)
        {
            var index = _items.IndexOf(item);
            if (index == -1)
            {
                return;
            }

            _items.RemoveAt(index);
            _currentItemIndex = Math.Min(index, _items.Count - 1);

            EmitSignal(SignalName.Changed);
        }

        public void SelectNext()
        {
            if (_currentItemIndex < 0)
            {
                return;
            }

            _currentItemIndex = (_currentItemIndex + 1) % _items.Count;
            EmitSignal(SignalName.Changed);
        }

        public void SelectPrev()
        {
            if (_currentItemIndex < 0)
            {
                return;
            }

            _currentItemIndex = (_currentItemIndex - 1 + _items.Count) % _items.Count;
            EmitSignal(SignalName.Changed);
        }
    }
}