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
                catch (Exception)
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
            if (!Engine.IsEditorHint() && this._<Game>().Flags.Has(GetFlag()))
            {
                QueueFree();
            }
        }

        protected override void _Interact()
        {
            base._Interact();
            this._<Game>().Flags.Add(GetFlag());
            this._<Game>().Inventory.AddItem((Item) _item);

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