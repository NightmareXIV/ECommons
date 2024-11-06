using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RetainerItemTransferList : AddonMasterBase<AddonRetainerItemTransferList>
    {
        public override string AddonDescription { get; } = "Entrust duplicates window";

        public RetainerItemTransferList(nint addon) : base(addon)
        {
        }

        public RetainerItemTransferList(void* addon) : base(addon)
        {
        }

        public void Confirm() => ClickButtonIfEnabled(Addon->ConfirmButton);
        public void Cancel() => ClickButtonIfEnabled(Addon->CancelButton);
    }
}
