using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Isoland.data;
using Isoland.items;
using Newtonsoft.Json;

namespace Isoland.globals
{
    public partial class Game : Node
    {
        private const string SavePath = "user://data.sav";
        
        [Signal]
        public delegate void ChangedEventHandler();

        public readonly FlagsClass Flags = new();
        public readonly InventoryClass Inventory = new();

        public void BackToTitle()
        {
            SaveGame();
            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.ChangeScene("res://ui/TitleScreen.tscn");
        }

        private GameData Write()
        {
            GameData.FlagsData flagsData = new()
            {
                Flags = Flags.Flags
            };

            List<string> items = new();
            if (Inventory.Items != null && Inventory.Items.Count > 0)
            {
                items.AddRange(Inventory.Items.Select(item => item.ResourcePath));
            }

            GameData.InventoryData inventoryData = new()
            {
                Items = items,
                CurrentItemIndex = Inventory.CurrentItemIndex
            };

            return new GameData
            {
                Flags = flagsData,
                Inventory = inventoryData,
                CurrentScene = GetTree().CurrentScene.SceneFilePath
            };
        }

        private void Read(GameData gameData)
        {
            var flagsData = gameData?.Flags;
            Flags.Flags = flagsData == null ? new List<string>() : flagsData.Flags;
            EmitSignal(FlagsClass.SignalName.Changed);

            var inventory = gameData?.Inventory ?? new GameData.InventoryData();
            Inventory.Items ??= new List<Item>();
            Inventory.Items.Clear();
            
            foreach (var item in inventory.Items)
            {
                var resource = GD.Load<Item>(item);
                Inventory.Items.Add(resource);
            }

            Inventory.CurrentItemIndex = inventory.CurrentItemIndex;
            EmitSignal(InventoryClass.SignalName.Changed);

            if (gameData?.CurrentScene == null || gameData.CurrentScene == string.Empty) return;
            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.ChangeScene(gameData.CurrentScene);
        }

        private void Reset()
        {
            Flags.Flags.Clear();
            EmitSignal(FlagsClass.SignalName.Changed);

            Inventory.Items.Clear();
            Inventory.CurrentItemIndex = -1;
            EmitSignal(InventoryClass.SignalName.Changed);
        }

        private void SaveGame()
        {
            var fileAccess = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
            if (fileAccess == null)
            {
                GD.Print($"[ERROR] Open file error! path: {SavePath}");
                return;
            }

            var gameData = Write();
            var json = JsonConvert.SerializeObject(gameData);
            fileAccess.StoreString(json);
            fileAccess.Dispose();
        }

        public void LoadGame()
        {
            var fileAccess = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            if (fileAccess == null)
            {
                GD.Print($"[ERROR] Open file error! path: {SavePath}");
                return;
            }
            
            var json = fileAccess.GetAsText();
            var gameData = JsonConvert.DeserializeObject<GameData>(json);
            Read(gameData);
            fileAccess.Dispose();
        }

        public void NewGame()
        {
            Reset();
            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.ChangeScene("res://scenes/H1.tscn");
        }

        public static bool HasSaveFile()
        {
            return FileAccess.FileExists(SavePath);
        }
    }

    public partial class FlagsClass : Node
    {
        [Signal]
        public delegate void ChangedEventHandler();

        private List<string> _flags = new();

        public List<string> Flags
        {
            get => _flags;
            set => _flags = value;
        }

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

        private List<Item> _items = new();

        public List<Item> Items
        {
            get => _items;
            set => _items = value;
        }

        private int _currentItemIndex = -1;

        public int CurrentItemIndex
        {
            get => _currentItemIndex;
            set => _currentItemIndex = value;
        }

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