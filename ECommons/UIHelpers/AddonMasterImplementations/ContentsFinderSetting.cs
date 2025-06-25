using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ContentsFinderSetting : AddonMasterBase<AtkUnitBase>
    {
        public ContentsFinderSetting(nint addon) : base(addon) { }
        public ContentsFinderSetting(void* addon) : base(addon) { }

        public AtkComponentButton* ConfirmButton => Addon->GetComponentButtonById(29);
        public AtkComponentButton* CloseButton => Addon->GetComponentButtonById(30);

        public override string AddonDescription { get; } = "Duty finder settings window";

        public bool Confirm() => ClickButtonIfEnabled(ConfirmButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
