using Godot;
using Isoland.globals;

namespace Isoland.ui
{
    public partial class TitleScreen : TextureRect
    {
        private Button _newGame;
        private Button _loadGame;
        private Button _quitGame;

        public override void _Ready()
        {
            base._Ready();
            var _newGame = GetNode<Button>("VBoxContainer/New");
            var _loadGame = GetNode<Button>("VBoxContainer/Load");
            var _quitGame = GetNode<Button>("VBoxContainer/Quit");
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
