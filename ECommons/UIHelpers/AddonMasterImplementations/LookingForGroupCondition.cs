using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroupCondition : AddonMasterBase
    {
        public LookingForGroupCondition(nint addon) : base(addon) { }

        public LookingForGroupCondition(void* addon) : base(addon) { }

        public AtkComponentButton* RecruitButton => Base->GetButtonNodeById(111);
        public AtkComponentButton* CancelButton => Base->GetButtonNodeById(112);
        public AtkComponentButton* ResetButton => Base->GetButtonNodeById(110);

        public override string AddonDescription { get; } = "Party finder creation window";

        public bool Normal() => ClickButtonIfEnabled(Base->GetButtonNodeById(3));
        public bool Alliance() => ClickButtonIfEnabled(Base->GetButtonNodeById(4));
        public bool Recruit() => ClickButtonIfEnabled(RecruitButton);
        public bool Cancel() => ClickButtonIfEnabled(CancelButton);
        public bool Reset() => ClickButtonIfEnabled(ResetButton);

        public void SelectDutyCategory(byte i)
        {
            var evt = new AtkEvent();
            var data = CreateAtkEventData().Write<byte>(16, i).Write<byte>(22, i).Build();
            Base->ReceiveEvent((AtkEventType)37, 7, &evt, &data);
        }
    }
}
