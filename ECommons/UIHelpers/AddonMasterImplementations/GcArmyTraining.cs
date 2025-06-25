using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class GcArmyTraining : AddonMasterBase<AtkUnitBase>
    {
        public GcArmyTraining(nint addon) : base(addon) { }
        public GcArmyTraining(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetComponentButtonById(39);

        public override string AddonDescription { get; } = "Squadron training window";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
