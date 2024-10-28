using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RecommendEquip : AddonMasterBase<AtkUnitBase>
    {
        public RecommendEquip(nint addon) : base(addon) { }
        public RecommendEquip(void* addon) : base(addon) { }

        public AtkComponentButton* EquipButton => Addon->GetButtonNodeById(11);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(12);

        public override string AddonDescription { get; } = "Equip recommended gear window";

        public void Equip() => ClickButtonIfEnabled(EquipButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
