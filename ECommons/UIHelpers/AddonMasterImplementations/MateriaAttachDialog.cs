using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MateriaAttachDialog : AddonMasterBase<AtkUnitBase>
    {
        public MateriaAttachDialog(nint addon) : base(addon)
        {
        }

        public MateriaAttachDialog(void* addon) : base(addon) { }

        public AtkComponentButton* MeldButton => Base->GetButtonNodeById(35);
        public AtkComponentButton* ReturnButton => Base->GetButtonNodeById(36);

        public void Meld() => ClickButtonIfEnabled(MeldButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}
