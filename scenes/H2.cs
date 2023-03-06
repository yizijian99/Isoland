using System.Collections.Generic;
using Godot;
using GodotUtilities;
using Isoland.globals;
using Isoland.items;
using Isoland.objects;
using Isoland.ui;

namespace Isoland.scenes
{
    public partial class H2 : Scene
    {
        [Node("Granny")]
        private Interactable _granny;

        [Node("Granny/DialogBubble")]
        private DialogBubble _dialogBubble;

        private static readonly List<string> Conversation = new()
        {
            "我年纪大了，很多事情想不起来了。",
            "你是谁？算了，我也不在乎你是谁。你能帮我找到信箱的钥匙吗？",
            "老头子说最近会给我寄船票过来，叫我和他一起出去看看。虽然我没有什么兴趣...",
            "他折腾了一辈子，不是躲在楼上捣鼓什么时间机器，就是出海找点什么东西。",
            "这些古怪的电视节目真没意思。",
            "老头子说这个岛上有很多秘密，其实我知道，不过是岛上的日子太孤独，他找点事情做罢了。",
            "人嘛，谁没有年轻过。年轻的时候...算了，不说这些往事了。",
            "老了才明白，万物静默如迷。"
        };

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

            _granny.Connect(Interactable.SignalName.Interact, Callable.From(OnGrannyInteract));
        }

        public override void _Process(double delta)
        {
        }

        private void OnGrannyInteract()
        {
            string _flag = "mail_accepted";

            var item = this._<Game>().Inventory.ActiveItem;
            if (item != null)
            {
                if (item == GD.Load<Item>("res://items/mail.tres"))
                {
                    this._<Game>().Flags.Add(_flag);
                    this._<Game>().Inventory.RemoveItem(item);
                }
                else
                {
                    return;
                }
            }

            if (this._<Game>().Flags.Has(_flag))
            {
                _dialogBubble.ShowDialog(new List<string>{"没想到老头子的船票寄过来了，谢谢你。"});
            }
            else
            {
                _dialogBubble.ShowDialog(Conversation);
            }
        }
    }
}