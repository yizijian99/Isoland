using System;
using Godot;

namespace Isoland.objects
{
    [Tool]
    [GodotClassName(nameof(SceneItem))]
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

        protected override void _Interact()
        {
            base._Interact();

            var sprite = new Sprite2D {Texture = (Texture2D) _item?.Get(items.Item.Const.SceneTexture)};
            GetParent().AddChild(sprite);
            sprite.GlobalPosition = GlobalPosition;

            var tween = sprite.CreateTween();
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
            tween.TweenProperty(sprite, (string) Node2D.PropertyName.Scale, Vector2.Zero, 0.15);
            tween.TweenCallback(Callable.From(() => sprite.QueueFree()));

            QueueFree();
        }
    }
}