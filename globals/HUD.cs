using Godot;
using Isoland.scenes;

namespace Isoland.globals
{
    public partial class HUD : CanvasLayer
    {
        private TextureButton _menu;

        public override void _Ready()
        {
            base._Ready();
            _menu = GetNode<TextureButton>("Menu");

            _menu.Connect(BaseButton.SignalName.Pressed, Callable.From(OnMenuPressed));

            var sceneChanger = GetNode<SceneChanger>($"/root/{nameof(SceneChanger)}");
            sceneChanger.Connect(SceneChanger.SignalName.GameEntered, Callable.From(Show));
            sceneChanger.Connect(SceneChanger.SignalName.GameExited, Callable.From(Hide));
            Visible = GetTree().CurrentScene is Scene;
        }

        private void OnMenuPressed()
        {
            var game = GetNode<Game>($"/root/{nameof(Game)}");
            game.BackToTitle();
        }
    }
}
