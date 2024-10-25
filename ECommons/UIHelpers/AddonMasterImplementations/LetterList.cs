using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class LetterList : AddonMasterBase<AtkUnitBase>
    {
        public LetterList(nint addon) : base(addon) { }
        public LetterList(void* addon) : base(addon) { }

        public AtkComponentButton* NewButton => Base->GetButtonNodeById(2);
        public AtkComponentButton* SentLetterHistoryButton => Base->GetButtonNodeById(3);
        public AtkComponentButton* DeliveryRequestButton => Base->GetButtonNodeById(4);

        public override string AddonDescription { get; } = "Mail list window";

        public void New() => ClickButtonIfEnabled(NewButton);
        public void SentLetterHistory() => ClickButtonIfEnabled(SentLetterHistoryButton);
        public void DeliveryRequest() => ClickButtonIfEnabled(DeliveryRequestButton);
    }
}
