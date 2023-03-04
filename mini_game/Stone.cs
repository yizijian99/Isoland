using System;
using Godot;
using Isoland.objects;

namespace Isoland.mini_game
{
    [Tool]
    public partial class Stone : Interactable
    {
        private H2AConfig.Slot _targetSlot;

        public H2AConfig.Slot TargetSlot
        {
            get => _targetSlot;
            set
            {
                _targetSlot = value;
                UpdateTexture();
            }
        }

        private H2AConfig.Slot _currentSlot;

        public H2AConfig.Slot CurrentSlot
        {
            get => _currentSlot;
            set
            {
                _currentSlot = value;
                UpdateTexture();
            }
        }

        private void UpdateTexture()
        {
            var index = (int) TargetSlot;
            if (TargetSlot != CurrentSlot)
            {
                index += Enum.GetValues<H2AConfig.Slot>().Length - 1;
            }
            
            TextureVariable = index == 0 ? null : GD.Load<Texture2D>($"res://assets/H2A/SS_{index:D2}.png");
        }
    }
}