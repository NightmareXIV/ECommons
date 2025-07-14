using FFXIVClientStructs.FFXIV.Component.GUI;
namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe partial class MJIHud : AddonMasterBase<AtkUnitBase>
    {
        public MJIHud(nint addon) : base(addon) { }
        public MJIHud(void* addon) : base(addon) { }

        public override string AddonDescription { get; } = "Island Sanctuary Main Hud";

        public uint SanctuaryRank => Addon->AtkValues[11].UInt;
        public uint CurrentIslandXP => Addon->AtkValues[12].UInt;
        public uint NextIslandLevelXP => Addon->AtkValues[13].UInt;
        public uint IslandersCowrie => Addon->AtkValues[14].UInt;
        public uint SeafarersCowrie => Addon->AtkValues[17].UInt;

        public AtkComponentButton* IsleventoryButton => Addon->GetComponentButtonById(20);
        public AtkComponentButton* SanctuaryCraftingLogButton => Addon->GetComponentButtonById(21);
        public AtkComponentButton* SanctuaryGatheringLogButton => Addon->GetComponentButtonById(22);
        public AtkComponentButton* ManageHideawayButton => Addon->GetComponentButtonById(23);
        public AtkComponentButton* MaterialAllocationButton => Addon->GetComponentButtonById(24);
        public AtkComponentButton* ManageMinionButton => Addon->GetComponentButtonById(25);
        public AtkComponentButton* ManageFurnishingButton => Addon->GetComponentButtonById(26);
        public AtkComponentButton* GuideButton => Addon->GetComponentButtonById(27);


        public void Isleventory() => ClickButtonIfEnabled(IsleventoryButton);
        public void SanctuaryCraftingLog() => ClickButtonIfEnabled(SanctuaryCraftingLogButton);
        public void SanctuaryGatheringLog() => ClickButtonIfEnabled(SanctuaryGatheringLogButton);
        public void ManageHideaway() => ClickButtonIfEnabled(ManageHideawayButton);
        public void MaterialAllocation() => ClickButtonIfEnabled(MaterialAllocationButton);
        public void ManageMinions() => ClickButtonIfEnabled(ManageHideawayButton);
        public void ManageFurnishing() => ClickButtonIfEnabled(ManageFurnishingButton);
        public void Guide() => ClickButtonIfEnabled(GuideButton);
    }
}
