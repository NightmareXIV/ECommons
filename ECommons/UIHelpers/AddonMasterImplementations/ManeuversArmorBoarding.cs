using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Rival Wings mount choice addon
    /// </summary>
    public unsafe class ManeuversArmorBoarding : AddonMasterBase<AtkUnitBase>
    {
        public ManeuversArmorBoarding(nint addon) : base(addon) { }
        public ManeuversArmorBoarding(void* addon) : base(addon) { }

        public AtkComponentButton* CruiseChaserButton => Addon->GetComponentButtonById(4);
        public AtkComponentButton* OppressorButton => Addon->GetComponentButtonById(5);
        public AtkComponentButton* BruteJusticeButton => Addon->GetComponentButtonById(6);
        public AtkComponentButton* MountButton => Addon->GetComponentButtonById(11);

        public override string AddonDescription { get; } = "Rival Wings mount choice window";

        public void CruiseChaser() => ClickButtonIfEnabled(CruiseChaserButton);
        public void Oppressor() => ClickButtonIfEnabled(OppressorButton);
        public void BruteJustice() => ClickButtonIfEnabled(BruteJusticeButton);
        public void Mount() => ClickButtonIfEnabled(MountButton);
    }
}
