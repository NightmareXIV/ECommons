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

        public AtkComponentButton* OpenDutyFinderButton => Addon->GetButtonNodeById(42);
        public AtkComponentButton* OpenAreaMapButton => Addon->GetButtonNodeById(43);
        public AtkComponentButton* RewardsButton => Addon->GetButtonNodeById(44);
        public AtkComponentButton* AccessSpecialSiteButton => Addon->GetButtonNodeById(59);

        public override string AddonDescription => "Mogpendium window";

        // TODO: claim rewards button

        public void OpenDutyFinder() => ClickButtonIfEnabled(OpenDutyFinderButton);
        public void OpenAreaMap() => ClickButtonIfEnabled(OpenAreaMapButton);
        public void Rewards() => ClickButtonIfEnabled(RewardsButton);
        public void AccessSpecialSite() => ClickButtonIfEnabled(AccessSpecialSiteButton);
    }
}
