using Godot;
using Isoland.globals;

namespace Isoland.objects
{
    [Tool]
    public partial class Teleporter : Interactable
    {
        [Export(PropertyHint.File, "*.tscn")] public string TargetPath;

        protected override void _Interact()
        {
            base._Interact();
            this._<SceneChanger>().ChangeScene(TargetPath);
        }
    }
}