using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Mogpendium addon
    /// </summary>
    public unsafe class MoogleCollection : AddonMasterBase<AtkUnitBase>
    {
        public MoogleCollection(nint addon) : base(addon) { }
        public MoogleCollection(void* addon) : base(addon) { }

        public AtkComponentButton* OpenDutyFinderButton => Addon->GetComponentButtonById(42);
        public AtkComponentButton* OpenAreaMapButton => Addon->GetComponentButtonById(43);
        public AtkComponentButton* RewardsButton => Addon->GetComponentButtonById(44);
        public AtkComponentButton* AccessSpecialSiteButton => Addon->GetComponentButtonById(59);

        public override string AddonDescription => "Mogpendium window";

        // TODO: claim rewards button

        public void OpenDutyFinder() => ClickButtonIfEnabled(OpenDutyFinderButton);
        public void OpenAreaMap() => ClickButtonIfEnabled(OpenAreaMapButton);
        public void Rewards() => ClickButtonIfEnabled(RewardsButton);
        public void AccessSpecialSite() => ClickButtonIfEnabled(AccessSpecialSiteButton);
    }
}
