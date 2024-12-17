using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MiragePrismPrismSetConvertC : AddonMasterBase<AtkUnitBase>
    {
        public MiragePrismPrismSetConvertC(nint addon) : base(addon) { }
        public MiragePrismPrismSetConvertC(void* addon) : base(addon) { }

        public AtkComponentCheckBox* StoreAsOutfitGlamourCheckBox => Addon->GetComponentNodeById(4)->GetAsAtkComponentCheckBox();
        public AtkComponentButton* YesButton => Addon->GetButtonNodeById(6);
        public AtkComponentButton* NoButton => Addon->GetButtonNodeById(7);

        public override string AddonDescription { get; } = "Outfit glamour creation confirmation";

        public void Yes() => ClickButtonIfEnabled(YesButton);
        public void No() => ClickButtonIfEnabled(NoButton);
    }
}
