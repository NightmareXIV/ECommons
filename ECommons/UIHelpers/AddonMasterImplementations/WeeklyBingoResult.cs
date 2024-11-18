using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class WeeklyBingoResult : AddonMasterBase<AtkUnitBase>
    {
        public WeeklyBingoResult(nint addon) : base(addon) { }
        public WeeklyBingoResult(void* addon) : base(addon) { }

        public AtkComponentButton* AcceptButton => Addon->GetButtonNodeById(93);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(94);

        public override string AddonDescription { get; } = "Jumbo Cactpot Results";

        public void Accept() => ClickButtonIfEnabled(AcceptButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
