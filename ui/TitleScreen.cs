using Godot;
using GodotUtilities;
using Isoland.globals;

namespace Isoland.ui
{
    public partial class TitleScreen : TextureRect
    {
        [Node("VBoxContainer/New")]
        private Button _newGame;
        [Node("VBoxContainer/Load")]
        private Button _loadGame;
        [Node("VBoxContainer/Quit")]
        private Button _quitGame;

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
            var game = GetNode<Game>($"/root/{nameof(Game)}");

            _newGame.Connect(BaseButton.SignalName.Pressed, Callable.From(OnNewPressed));
            _loadGame.Connect(BaseButton.SignalName.Pressed, Callable.From(OnLoadPressed));
            _quitGame.Connect(BaseButton.SignalName.Pressed, Callable.From(OnQuitPressed));

            _loadGame.Disabled = !Game.HasSaveFile();
        }

        private void OnNewPressed()
        {
            var game = GetNode<Game>($"/root/{nameof(Game)}");
            game.NewGame();
        }

        private void OnLoadPressed()
        {
            var game = GetNode<Game>($"/root/{nameof(Game)}");
            game.LoadGame();
        }

        private void OnQuitPressed()
        {
            GetTree().Quit();
        }
    }
}
