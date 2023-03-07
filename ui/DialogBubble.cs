using System.Collections.Generic;
using Godot;
using GodotUtilities;

namespace Isoland.ui
{
    public partial class DialogBubble : Control
    {
        private const int InitLine = -1;

        [Node("Content")]
        private Label _content;

        private List<string> _dialogs = new();

        private int _currentLine = InitLine;

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
            _content.GuiInput += OnContentGuiInput;

            Hide();
        }

        public void ShowDialog(List<string> dialogs)
        {
            if (_currentLine == -1 || _dialogs != dialogs)
            {
                _dialogs = dialogs;
                ShowLine(0);
                Show();
            }
            else
            {
                Advance();
            }
        }

        private void ShowLine(int line)
        {
            _currentLine = line;
            _content.Text = _dialogs[_currentLine];

            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(this, "scale", Vector2.One, 0.2)
                .From(Vector2.Zero);
        }

        private void Advance()
        {
            var nextLine = _currentLine + 1;
            if (nextLine < _dialogs.Count)
            {
                _currentLine = nextLine;
                ShowLine(_currentLine);
            }
            else
            {
                _currentLine = InitLine;
                Hide();
            }
        }

        // TODO GuiInput信号不生效
        private void OnContentGuiInput(InputEvent @event)
        {
            if (@event.IsActionPressed("interact"))
            {
                Advance();
            }
        }
    }
}