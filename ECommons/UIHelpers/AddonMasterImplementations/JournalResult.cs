using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class JournalResult : AddonMasterBase<AddonJournalResult>
    {
        public JournalResult(nint addon) : base(addon)
        {
        }

        public JournalResult(void* addon) : base(addon) { }

        public override string AddonDescription { get; } = "Quest completion window";

        public void Complete() => ClickButtonIfEnabled(Addon->CompleteButton);
        public void Decline() => ClickButtonIfEnabled(Addon->DeclineButton);
    }
}
