using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageResult : AddonMasterBase<AtkUnitBase>
    {
        public SalvageResult(nint addon) : base(addon) { }

        public SalvageResult(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(15);

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
