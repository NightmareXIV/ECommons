using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroupDetail : AddonMasterBase<AddonLookingForGroupDetail>
    {
        public LookingForGroupDetail(nint addon) : base(addon) { }
        public LookingForGroupDetail(void* addon) : base(addon) { }

        public AtkComponentButton* JoinEditButton => Base->GetButtonNodeById(109);
        public AtkComponentButton* TellEndButton => Base->GetButtonNodeById(110);
        public AtkComponentButton* BackButton => Base->GetButtonNodeById(111);

        public bool JoinEdit() => ClickButtonIfEnabled(JoinEditButton);
        public bool TellEnd() => ClickButtonIfEnabled(TellEndButton);
        public bool Back() => ClickButtonIfEnabled(BackButton);

        public string PartyLeader => Addon->GetTextNodeById(6)->NodeText.GetText();
        public string Description => Addon->GetTextNodeById(20)->NodeText.GetText();
        public string World => Addon->GetTextNodeById(33)->NodeText.GetText();

        public override string AddonDescription { get; } = "Party finder details window";
    }
}