using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RetainerItemTransferProgress : AddonMasterBase<AddonRetainerItemTransferProgress>
    {
        public RetainerItemTransferProgress(nint addon) : base(addon)
        {
        }

        public RetainerItemTransferProgress(void* addon) : base(addon)
        {
        }

        public void Close() => ClickButtonIfEnabled(Addon->CloseWindowButton);
    }
}
