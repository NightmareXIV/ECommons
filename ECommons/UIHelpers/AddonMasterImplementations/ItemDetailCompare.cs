using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ItemDetailCompare : AddonMasterBase<AtkUnitBase>
    {
        public ItemDetailCompare(nint addon) : base(addon) { }
        public ItemDetailCompare(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetComponentButtonById(148);

        public override string AddonDescription { get; } = "Item Comparison";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}