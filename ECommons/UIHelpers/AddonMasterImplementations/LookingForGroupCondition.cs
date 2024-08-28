using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroupCondition : AddonMasterBase
    {
        public LookingForGroupCondition(nint addon) : base(addon)
        {
        }

        public LookingForGroupCondition(void* addon) : base(addon)
        {
        }

        public bool Normal() => ClickButtonIfEnabled(this.Base->GetButtonNodeById(3));
        public bool Alliance() => ClickButtonIfEnabled(this.Base->GetButtonNodeById(4));
        public bool Recruit() => ClickButtonIfEnabled(this.Base->GetButtonNodeById(111));

        public void SelectDutyCategory(byte i)
        {
            var evt = new AtkEvent();
            var data = this.CreateAtkEventData().Write<byte>(16, i).Write<byte>(22, i).Build();
            this.Base->ReceiveEvent((AtkEventType)37, 7, &evt, &data);
        }

        public void Reset() => ClickButtonIfEnabled(this.Base->GetButtonNodeById(110));
    }
}
