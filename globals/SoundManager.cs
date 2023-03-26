using Godot;
using GodotUtilities;

namespace Isoland.globals
{
    public partial class SoundManager : Node
    {
        [Node("BGMPlayer")]
        private AudioStreamPlayer _bgmPlayer;

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
        }

        public void PlayMusic(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (_bgmPlayer.Playing && _bgmPlayer.Stream.ResourcePath == path)
            {
                return;
            }

            _bgmPlayer.Stream = GD.Load<AudioStream>(path);
            _bgmPlayer.Play();
        }
    }
}