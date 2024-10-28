using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ItemFinder : AddonMasterBase<AtkUnitBase>
    {
        public ItemFinder(nint addon) : base(addon) { }
        public ItemFinder(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(14);

        public override string AddonDescription { get; } = "Item search window";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
