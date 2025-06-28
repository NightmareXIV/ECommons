using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MiragePrismExecute : AddonMasterBase<AtkUnitBase>
    {
        public MiragePrismExecute(nint addon) : base(addon) { }
        public MiragePrismExecute(void* addon) : base(addon) { }

        public AtkComponentButton* CastButton => Addon->GetComponentButtonById(24);
        public AtkComponentButton* ReturnButton => Addon->GetComponentButtonById(25);

        public override string AddonDescription { get; } = "Cast glamour window";

        public void Cast() => ClickButtonIfEnabled(CastButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}
