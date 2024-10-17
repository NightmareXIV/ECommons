using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Title screen error code addon
    /// </summary>
    public unsafe class Dialogue : AddonMasterBase<AtkUnitBase>
    {
        public Dialogue(nint addon) : base(addon) { }
        public Dialogue(void* addon) : base(addon) { }

        public AtkComponentButton* OkButton => Addon->GetButtonNodeById(4);

        public void Ok() => ClickButtonIfEnabled(OkButton);
    }
}
