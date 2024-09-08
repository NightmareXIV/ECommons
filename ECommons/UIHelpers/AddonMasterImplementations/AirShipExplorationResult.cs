using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class AirShipExplorationResult : AddonMasterBase<AtkUnitBase>
    {
        public AirShipExplorationResult(nint addon) : base(addon) { }

        public AirShipExplorationResult(void* addon) : base(addon) { }

        public AtkComponentButton* RedeployButton => Addon->GetButtonNodeById(47);
        public AtkComponentButton* FinalizeReportButton => Addon->GetButtonNodeById(48);

        public void Redeploy() => ClickButtonIfEnabled(RedeployButton);
        public void FinalizeReport() => ClickButtonIfEnabled(FinalizeReportButton);
    }
}
