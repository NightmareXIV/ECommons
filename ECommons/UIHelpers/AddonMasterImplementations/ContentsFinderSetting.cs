using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ContentsFinderSetting : AddonMasterBase<AtkUnitBase>
    {
        public ContentsFinderSetting(nint addon) : base(addon) { }
        public ContentsFinderSetting(void* addon) : base(addon) { }

        public AtkComponentButton* ConfirmButton => Addon->GetButtonNodeById(29);
        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(30);

        public bool Confirm() => ClickButtonIfEnabled(ConfirmButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
