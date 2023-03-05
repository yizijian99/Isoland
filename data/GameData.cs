using System.Collections.Generic;

namespace Isoland.data
{
    public class GameData
    {
        public FlagsData Flags = new();
        public InventoryData Inventory = new();
        public string CurrentScene;

        public class FlagsData
        {
            public List<string> Flags = new();
        }

        public class InventoryData
        {
            public List<string> Items = new();
            public int CurrentItemIndex = -1;
        }
    }
}