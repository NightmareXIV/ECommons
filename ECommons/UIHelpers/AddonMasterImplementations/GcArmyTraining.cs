using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class GcArmyTraining : AddonMasterBase<AtkUnitBase>
    {
        public GcArmyTraining(nint addon) : base(addon) { }
        public GcArmyTraining(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(39);

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
