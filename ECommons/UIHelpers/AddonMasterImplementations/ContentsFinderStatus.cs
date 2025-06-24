using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ContentsFinderStatus : AddonMasterBase<AtkUnitBase>
    {
        public ContentsFinderStatus(nint addon) : base(addon) { }
        public ContentsFinderStatus(void* addon) : base(addon) { }

        public AtkComponentButton* WithdrawButton => Addon->GetComponentButtonById(40);

        public override string AddonDescription => "Duty finder status window";

        public void Withdraw() => ClickButtonIfEnabled(WithdrawButton);
    }
}
