using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Isoland.mini_game
{
    [Tool]
    public partial class H2AConfig : Resource
    {
        public enum Slot
        {
            Null,
            Time,
            Sun,
            Fish,
            Hill,
            Cross,
            Choice
        }

        private static class Const
        {
            // custom property name constant
            public const string Placements = nameof(_placements);
            public const string Connections = nameof(_connections);

            // custom property prefix constant
            public static readonly string PlacementsPrefix = $"{Placements}/";
            public static readonly string ConnectionsPrefix = $"{Connections}/";

            // property key constant
            public const string Name = "name";
            public const string Type = "type";
            public const string Usage = "usage";
            public const string Hint = "hint";
            public const string HintString = "hint_string";
        }

        private Array<Slot> _placements = new();
        private Dictionary<Slot, Array<Slot>> _connections = new();

        public H2AConfig()
        {
            var slots = Enum.GetValues<Slot>();
            _placements.Resize(slots.Length);
            for (var i = 0; i < slots.Length; i++)
            {
                _placements[i] = Slot.Null;
                _connections.Add(slots[i], new Array<Slot>());
            }
        }

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = new()
            {
                new Dictionary
                {
                    {Variant.From(Const.Name), Variant.From(Const.Placements)},
                    {Variant.From(Const.Type), Variant.From(Variant.Type.PackedInt32Array)},
                    {Variant.From(Const.Usage), Variant.From(PropertyUsageFlags.Storage)}
                },
                new Dictionary
                {
                    {Variant.From(Const.Name), Variant.From(Const.Connections)},
                    {Variant.From(Const.Type), Variant.From(Variant.Type.Dictionary)},
                    {Variant.From(Const.Usage), Variant.From(PropertyUsageFlags.Storage)}
                }
            };

            for (var i = 1; i < Enum.GetValues<Slot>().Length; i++)
            {
                properties.Add(new Dictionary
                {
                    {Variant.From(Const.Name), Variant.From($"{Const.PlacementsPrefix}{Enum.GetName((Slot) i)}")},
                    {Variant.From(Const.Type), Variant.From(Variant.Type.Int)},
                    {Variant.From(Const.Usage), Variant.From(PropertyUsageFlags.Editor)},
                    {Variant.From(Const.Hint), Variant.From(PropertyHint.Enum)},
                    {Variant.From(Const.HintString), Variant.From($"{Enum.GetNames(typeof(Slot)).Join(",")}")}
                });
            }

            for (var i = 0; i < Enum.GetValues<Slot>().Length - 1; i++)
            {
                var values = Enum.GetValues(typeof(Slot));
                var available = (from object dst in values
                    select (int) dst <= i ? string.Empty : Enum.GetName(typeof(Slot), dst)).ToList();

                properties.Add(new Dictionary
                {
                    {Variant.From(Const.Name), Variant.From($"{Const.ConnectionsPrefix}{Enum.GetName((Slot) i)}")},
                    {Variant.From(Const.Type), Variant.From(Variant.Type.Int)},
                    {Variant.From(Const.Usage), Variant.From(PropertyUsageFlags.Editor)},
                    {Variant.From(Const.Hint), Variant.From(PropertyHint.Flags)},
                    {Variant.From(Const.HintString), Variant.From($"{string.Join(',', available)}")}
                });
            }

            return properties;
        }

        public override Variant _Get(StringName property)
        {
            if (property == null)
            {
                return Variant.From<Dictionary>(null);
            }

            var propertyString = property.ToString();
            if (propertyString.StartsWith(Const.PlacementsPrefix))
            {
                propertyString = propertyString.TrimStart(Const.PlacementsPrefix.ToCharArray());
                var index = (int) Enum.Parse(typeof(Slot), propertyString);
                return Variant.From(_placements[index]);
            }

            if (propertyString.StartsWith(Const.ConnectionsPrefix))
            {
                propertyString = propertyString.TrimStart(Const.ConnectionsPrefix.ToCharArray());
                var slot = (Slot) Enum.Parse(typeof(Slot), propertyString);
                var index = (int) slot;
                var value = 0;
                var values = Enum.GetValues<Slot>();
                for (var i = index + 1; i < values.Length; i++)
                {
                    var dst = values[i];
                    if (_connections[slot].Contains(dst))
                    {
                        value |= 1 << (int) dst;
                    }
                }

                return value;
            }

            return Variant.From<Dictionary>(null);
        }

        public override bool _Set(StringName property, Variant value)
        {
            if (property == null)
            {
                return false;
            }

            var propertyString = property.ToString();
            if (propertyString.StartsWith(Const.PlacementsPrefix))
            {
                propertyString = propertyString.TrimStart(Const.PlacementsPrefix.ToCharArray());
                var index = (int) Enum.Parse(typeof(Slot), propertyString);
                _placements[index] = value.As<Slot>();
                EmitChanged();
                return true;
            }
            
            if (propertyString.StartsWith(Const.ConnectionsPrefix))
            {
                propertyString = propertyString.TrimStart(Const.ConnectionsPrefix.ToCharArray());
                var index = (int) Enum.Parse(typeof(Slot), propertyString);
                var values = Enum.GetValues<Slot>();
                for (var i = index + 1; i < values.Length; i++)
                {
                    var dst = (int) values[i];
                    SetConnected(index, dst, ((value.AsInt32() >> dst) & 1) == 1);
                }
                EmitChanged();
                return true;
            }

            return false;
        }

        private void SetConnected(int src, int dst, bool connected)
        {
            var slots = Enum.GetValues<Slot>();
            var scrArr = _connections[slots[src]];
            var dstArray = _connections[slots[dst]];
            var srcIdx = scrArr.IndexOf(slots[dst]);
            var dstIdx = scrArr.IndexOf(slots[src]);
            if (connected)
            {
                if (srcIdx == -1)
                {
                    scrArr.Add(slots[dst]);
                }

                if (dstIdx == -1)
                {
                    dstArray.Add(slots[src]);
                }
            }
            else
            {
                if (srcIdx != -1)
                {
                    scrArr.RemoveAt(srcIdx);
                }

                if (dstIdx != -1)
                {
                    dstArray.RemoveAt(dstIdx);
                }
            }
        }
    }
}