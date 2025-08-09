using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MiragePrismRemove : AddonMasterBase<AtkUnitBase>
    {
        public MiragePrismRemove(nint addon) : base(addon) { }
        public MiragePrismRemove(void* addon) : base(addon) { }

        public AtkComponentButton* DispelButton => Addon->GetComponentButtonById(15);
        public AtkComponentButton* ReturnButton => Addon->GetComponentButtonById(16);

        public override string AddonDescription { get; } = "Remove glamour window";

        public void Dispel() => ClickButtonIfEnabled(DispelButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}
