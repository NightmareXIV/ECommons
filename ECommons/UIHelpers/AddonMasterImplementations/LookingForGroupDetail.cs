using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroupDetail : AddonMasterBase
    {
        public LookingForGroupDetail(nint addon) : base(addon) { }

        public LookingForGroupDetail(void* addon) : base(addon) { }

        public AtkComponentButton* EditButton => Base->GetButtonNodeById(109);
        public AtkComponentButton* EndButton => Base->GetButtonNodeById(110);
        public AtkComponentButton* BackButton => Base->GetButtonNodeById(111);

        public bool Recruit() => ClickButtonIfEnabled(EditButton);
        public bool Cancel() => ClickButtonIfEnabled(EndButton);
        public bool Reset() => ClickButtonIfEnabled(BackButton);

        public string PartyLeader => Addon->GetTextNodeById(6)->NodeText.ExtractText();
        public string Description => Addon->GetTextNodeById(20)->NodeText.ExtractText();
        public string World => Addon->GetTextNodeById(33)->NodeText.ExtractText();
    }
}