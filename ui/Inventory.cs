using Godot;
using Isoland.globals;
using Isoland.items;

namespace Isoland.ui
{
    [Tool]
    public partial class Inventory : VBoxContainer
    {
        /*TODO 改变Inventory的尺寸时如何让其向左上角生长*/
        /*TODO _label.Hide(); _label.Show(); 暂时先注释掉，只是用透明度实现道具label消失与显示效果。道具label隐藏时道具栏会上移*/

        private Game _game;

        private Label _label;
        private TextureButton _prev;
        private TextureButton _use;
        private TextureButton _next;
        private Sprite2D _prop;
        private Sprite2D _hand;
        private Timer _timer;

        private Tween _handOutro;
        private Tween _labelOutro;

        public override void _Ready()
        {
            _game = GetNode<Game>($"/root/{nameof(Game)}");
            
            _label = GetNode<Label>("Label");
            _prev = GetNode<TextureButton>("ItemBar/Prev");
            _use = GetNode<TextureButton>("ItemBar/Use");
            _next = GetNode<TextureButton>("ItemBar/Next");
            _prop = GetNode<Sprite2D>("ItemBar/Use/Prop");
            _hand = GetNode<Sprite2D>("ItemBar/Use/Hand");
            _timer = GetNode<Timer>("Label/Timer");
            
            _game.Inventory.Connect(Game.SignalName.Changed, Callable.From(() => UpdateUi()));
            _prev.Connect(BaseButton.SignalName.Pressed, Callable.From(OnPrevPressed));
            _use.Connect(BaseButton.SignalName.Pressed, Callable.From(OnUsePressed));
            _next.Connect(BaseButton.SignalName.Pressed, Callable.From(OnNextPressed));
            _timer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimerTimeOut));

            _hand.Hide();
            var handModulate = _hand.Modulate;
            handModulate.A = 0.0f;
            _hand.Modulate = handModulate;
            
            // _label.Hide();
            var labelModulate = _label.Modulate;
            labelModulate.A = 0.0f;
            _label.Modulate = labelModulate;

            UpdateUi(true);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("interact") && _game.Inventory.ActiveItem != null)
            {
                _game.Inventory.SetDeferred(nameof(Game.Inventory.ActiveItem), Variant.From<Item>(null));

                _handOutro = CreateTween();
                _handOutro.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine).SetParallel();
                _handOutro.TweenProperty(_hand, "scale", Vector2.One * 3, 0.15);
                _handOutro.TweenProperty(_hand, "modulate:a", 0.0f, 0.15);
                _handOutro.Chain().TweenCallback(Callable.From(() => _hand.Hide()));
            }
        }

        private void UpdateUi(bool isInit = false)
        {
            var count = _game.Inventory.GetItemCount();
            _prev.Disabled = count < 2;
            _next.Disabled = count < 2;
            Visible = count > 0;

            var item = _game.Inventory.GetCurrentItem();
            if (item == null)
            {
                return;
            }

            _label.Text = item.Description;
            _prop.Texture = item.PropTexture as Texture2D;

            if (isInit) return;
            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
            tween.TweenProperty(_prop, "scale", Vector2.One, 0.15).From(Vector2.Zero);
            
            ShowLabel();
        }

        private void ShowLabel()
        {
            if (_labelOutro != null && _labelOutro.IsValid())
            {
                _labelOutro.Kill();
                _labelOutro = null;
            }
            // _label.Show();
            _labelOutro = CreateTween();
            _labelOutro.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            _labelOutro.TweenProperty(_label, "modulate:a", 1.0f, 0.2);
            _labelOutro.TweenCallback(Callable.From(() => _timer.Start()));
        }

        private void OnPrevPressed()
        {
            _game.Inventory.SelectPrev();
        }

        private void OnNextPressed()
        {
            _game.Inventory.SelectNext();
        }

        private void OnUsePressed()
        {
            _game.Inventory.ActiveItem = _game.Inventory.GetCurrentItem();
            if (_handOutro != null && _handOutro.IsValid())
            {
                _handOutro.Kill();
                _handOutro = null;
            }
            _hand.Show();
            ShowLabel();
            var tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel();
            tween.TweenProperty(_hand, "scale", Vector2.One, 0.15).From(Vector2.Zero);
            tween.TweenProperty(_hand, "modulate:a", 1.0f, 0.15);
        }

        private void OnTimerTimeOut()
        {
            _labelOutro = CreateTween();
            _labelOutro.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            _labelOutro.TweenProperty(_label, "modulate:a", 0.0f, 0.2);
            // _labelOutro.TweenCallback(Callable.From(() => _label.Hide()));
        }
    }
}