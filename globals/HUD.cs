using Godot;
using GodotUtilities;
using Isoland.scenes;

namespace Isoland.globals
{
    public partial class HUD : CanvasLayer
    {
        [Node("Menu")]
        private TextureButton _menu;

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

            _menu.Pressed += OnMenuPressed;

            this._<SceneChanger>().GameEntered += Show;
            this._<SceneChanger>().GameExited += Hide;
            Visible = GetTree().CurrentScene is Scene;
        }

        private void OnMenuPressed()
        {
            this._<Game>().BackToTitle();
        }
    }
}
