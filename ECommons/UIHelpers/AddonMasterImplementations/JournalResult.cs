using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class JournalResult : AddonMasterBase<AddonJournalResult>
    {
        public JournalResult(nint addon) : base(addon)
        {
        }

        public JournalResult(void* addon) : base(addon) { }

        public void Complete() => ClickButtonIfEnabled(Addon->CompleteButton);
        public void Decline() => ClickButtonIfEnabled(Addon->DeclineButton);
    }
}
