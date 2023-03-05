using Godot;

namespace Isoland.globals
{
    public partial class SoundManager : Node
    {
        private AudioStreamPlayer _bgmPlayer;

        public override void _Ready()
        {
            base._Ready();
            _bgmPlayer = GetNode<AudioStreamPlayer>("BGMPlayer");
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