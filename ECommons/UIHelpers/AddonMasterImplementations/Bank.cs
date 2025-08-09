using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Gil transfer addon (seen in retainers)
    /// </summary>
    public unsafe class Bank : AddonMasterBase<AtkUnitBase>
    {
        public Bank(nint addon) : base(addon) { }
        public Bank(void* addon) : base(addon) { }

        public AtkComponentButton* ProceedButton => Addon->GetComponentButtonById(35);
        public AtkComponentButton* CancelButton => Addon->GetComponentButtonById(36);

        public override string AddonDescription { get; } = "Gil transfer window";

        public void Proceed() => ClickButtonIfEnabled(ProceedButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);

    }
}
