using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageResult : AddonMasterBase<AtkUnitBase>
    {
        public SalvageResult(nint addon) : base(addon) { }

        public SalvageResult(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetComponentButtonById(15);

        public override string AddonDescription { get; } = "Desynthesis result window";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
