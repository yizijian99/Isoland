using Godot;
using Isoland.globals;

namespace Isoland.objects
{
    public partial class Teleporter : Interactable
    {
        [Export(PropertyHint.File, "*.tscn")] public string TargetPath;

        private SceneChanger _sceneChanger;

        public override void _Ready()
        {
            _sceneChanger = GetNode<SceneChanger>("SceneChanger");
        }

        protected override void _Interact()
        {
            base._Interact();
            _sceneChanger.ChangeScene(TargetPath);
        }
    }
}
