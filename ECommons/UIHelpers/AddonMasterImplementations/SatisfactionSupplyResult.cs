using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class SatisfactionSupplyResult : AddonMasterBase<AtkUnitBase>
    {
        public SatisfactionSupplyResult(nint addon) : base(addon) { }
        public SatisfactionSupplyResult(void* addon) : base(addon) { }
        public override string AddonDescription { get; } = "Custom Deliveries Rank Up Window";

        public uint CurrentLevelXp => Addon->AtkValues[10].UInt;
        public uint TotalLevelXp => Addon->AtkValues[11].UInt;

        public AtkComponentButton* AcceptButton => Addon->GetButtonNodeById(36);

        public void Accept() => ClickButtonIfEnabled(AcceptButton);
    }
}
