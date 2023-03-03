using System;
using Godot;
using Isoland.globals;
using Isoland.items;

namespace Isoland.objects
{
    [Tool]
    public partial class SceneItem : Interactable
    {
        private Resource _item;

        public Game Game;

        [Export]
        public Resource Item
        {
            get => _item;
            set
            {
                _item = value;
                try
                {
                    TextureVariable = (Texture2D) _item?.Get(items.Item.Const.SceneTexture);
                }
                catch (Exception e)
                {
                    _item = null;
                }
                finally
                {
                    if (_item == null)
                    {
                        TextureVariable = null;
                    }

                    NotifyPropertyListChanged();
                }
            }
        }

        public override void _Ready()
        {
            Game = GetNode<Game>($"/root/{nameof(Game)}");

            if (!Engine.IsEditorHint() && Game.Flags.Has(GetFlag()))
            {
                // TODO 拾取道具后，二次进入场景时，已拾取的道具会闪烁
                QueueFree();
            }
        }

        protected override void _Interact()
        {
            base._Interact();
            Game.Flags.Add(GetFlag());
            Game.Inventory.AddItem((Item) _item);

            var sprite = new Sprite2D {Texture = (Texture2D) _item?.Get(items.Item.Const.SceneTexture)};
            GetParent().AddChild(sprite);
            sprite.GlobalPosition = GlobalPosition;

            var tween = sprite.CreateTween();
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
            tween.TweenProperty(sprite, (string) Node2D.PropertyName.Scale, Vector2.Zero, 0.15);
            tween.TweenCallback(Callable.From(() => sprite.QueueFree()));

            QueueFree();
        }

        private string GetFlag()
        {
            return $"picked:{_item.ResourcePath.GetFile()}";
        }
    }
}