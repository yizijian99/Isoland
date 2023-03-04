using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Isoland.globals;
using Isoland.objects;

namespace Isoland.mini_game
{
    [Tool]
    public partial class Board : Node2D
    {
        private static Texture2D _slotTexture;
        private static Texture2D _lineTexture;

        [Export] public float Radius = 350.0f;
        
        private Resource _config;

        [Export]
        public Resource Config
        {
            get => _config;
            set
            {
                if (_config != null && _config.IsConnected(Resource.SignalName.Changed, Callable.From(UpdateBoard)))
                {
                    _config.Disconnect(Resource.SignalName.Changed, Callable.From(UpdateBoard));
                }
                _config = value;
                if (_config != null && !_config.IsConnected(Resource.SignalName.Changed, Callable.From(UpdateBoard)))
                {
                    _config.Connect(Resource.SignalName.Changed, Callable.From(UpdateBoard));
                }
                UpdateBoard();
            }
        }

        private readonly System.Collections.Generic.Dictionary<H2AConfig.Slot, Stone> _stoneMap = new();

        private Game _game;

        public Board()
        {
            _slotTexture = GD.Load<Texture2D>("res://assets/H2A/CIRCLE.png");
            _lineTexture = GD.Load<Texture2D>("res://assets/H2A/CIRCLELINE.png");
        }

        public override void _Ready()
        {
            base._Ready();
            _game = GetNode<Game>($"/root/{nameof(Game)}");
            
            UpdateBoard();
        }

        public override void _Draw()
        {
            base._Draw();
            var slots = Enum.GetValues<H2AConfig.Slot>();
            foreach (var slot in slots)
            {
                DrawTexture(_slotTexture, GetSlotPosition(slot) - _slotTexture.GetSize() / 2);
            }
        }

        private Vector2 GetSlotPosition(H2AConfig.Slot slot)
        {
            var slots = Enum.GetValues<H2AConfig.Slot>();
            return Vector2.Down.Rotated(Mathf.Tau / slots.Length * (int) slot) * Radius;
        }

        private void UpdateBoard()
        {
            var children = GetChildren();
            foreach (var node in children)
            {
                if (node.Owner == null)
                {
                    node.QueueFree();
                }
            }

            if (_config == null)
            {
                return;
            }

            var connections =
                (Godot.Collections.Dictionary<H2AConfig.Slot, Array<H2AConfig.Slot>>) _config.Get("_connections");
            if (connections == null)
            {
                return;
            }

            var placement = (Array<H2AConfig.Slot>) _config.Get("_placements");
            if (placement == null || placement.Count == 0)
            {
                return;
            }

            var slots = Enum.GetValues<H2AConfig.Slot>();
            foreach (var src in slots)
            {
                var slot = (int) src;
                for (var i = slot + 1; i < slots.Length; i++)
                {
                    var dst = slots[i];
                    if (!connections[src].Contains(dst))
                    {
                        continue;
                    }

                    Line2D line = new();
                    line.AddPoint(GetSlotPosition(src));
                    line.AddPoint(GetSlotPosition(dst));
                    line.Width = _lineTexture.GetSize().Y;
                    line.Texture = _lineTexture;
                    line.TextureMode = Line2D.LineTextureMode.Tile;
                    line.DefaultColor = Colors.White;
                    line.ShowBehindParent = true;
                    AddChild(line);
                }
            }
            
            foreach (var slot in slots)
            {
                if (slot == H2AConfig.Slot.Null)
                {
                    continue;
                }

                Stone stone = new();
                stone.TargetSlot = slot;
                stone.CurrentSlot = placement[(int) slot];
                stone.Position = GetSlotPosition(stone.CurrentSlot);
                _stoneMap[slot] = stone;
                AddChild(stone);
                stone.Connect(Interactable.SignalName.Interact, Callable.From(() => RequestMove(stone)));
            }
        }

        private void RequestMove(Stone stone)
        {
            var available = new List<H2AConfig.Slot>(Enum.GetValues<H2AConfig.Slot>());
            foreach (var value in _stoneMap.Values)
            {
                available.Remove(value.CurrentSlot);
            }
            
            
            if (available.Count == 1)
            {
                var availableSlot = available[0];
                var connections = (Godot.Collections.Dictionary<H2AConfig.Slot, Array<H2AConfig.Slot>>) _config.Get("_connections");
                if (connections[stone.CurrentSlot].Contains(availableSlot))
                {
                    MoveStone(stone, availableSlot);
                }
            }
            else
            {
                GD.Print($"[WARN] data error.");
            }
        }

        private void MoveStone(Stone stone, H2AConfig.Slot targetSlot)
        {
            stone.CurrentSlot = targetSlot;
            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(stone, "position", GetSlotPosition(targetSlot), 0.2);
            tween.TweenInterval(1.0);
            tween.TweenCallback(Callable.From(Check));
        }

        private void Check()
        {
            if (_stoneMap.Values.Any(stone => stone.CurrentSlot != stone.TargetSlot))
            {
                return;
            }
            
            _game.Flags.Add("h2a_unlocked");
            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.ChangeScene("res://scenes/H2.tscn");
        }
    }
}