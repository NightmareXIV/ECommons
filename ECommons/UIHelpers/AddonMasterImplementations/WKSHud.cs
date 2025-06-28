using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Text.RegularExpressions;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Space Exploration main window (always visible on the area)
    /// </summary>
    public unsafe partial class WKSHud : AddonMasterBase<AtkUnitBase>
    {
        public WKSHud(nint addon) : base(addon) { }
        public WKSHud(void* addon) : base(addon) { }

        public AtkComponentButton* StellerMissionsButton => Addon->GetComponentButtonById(6);
        public AtkComponentButton* MechOpsButton => Addon->GetComponentButtonById(7);
        public AtkComponentButton* StellerSuccessButton => Addon->GetComponentButtonById(8);
        public AtkComponentButton* InfrastructorIndexButton => Addon->GetComponentButtonById(9);
        public AtkComponentButton* CosmicResearchButton => Addon->GetComponentButtonById(10);
        public AtkComponentButton* CosmicClassTrackerButton => Addon->GetComponentButtonById(11);

        public int CosmoCredit => Addon->AtkValues[2].Int;
        public int LunarCredit => Addon->AtkValues[6].Int;

        public override string AddonDescription => "Cosmic Exploration Main Hud Menu";

        public void Mission() => ClickButtonIfEnabled(StellerMissionsButton);
        public void Mech() => ClickButtonIfEnabled(MechOpsButton);
        public void Steller() => ClickButtonIfEnabled(StellerSuccessButton);
        public void Infrastructor() => ClickButtonIfEnabled(InfrastructorIndexButton);
        public void Research() => ClickButtonIfEnabled(CosmicResearchButton);
        public void ClassTracker() => ClickButtonIfEnabled(CosmicClassTrackerButton);

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumber();
    }
}
