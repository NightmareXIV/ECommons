using ECommons.Automation;
using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Normal() => this.Base->GetButtonNodeById(3)->ClickAddonButton(this.Base);
        public void Alliance() => this.Base->GetButtonNodeById(4)->ClickAddonButton(this.Base);

        public void SelectDutyCategory(byte i)
        {
            var evt = new AtkEvent();
            var data = this.CreateAtkEventData().Write<byte>(16, i).Write<byte>(22, i).Build();
            this.Base->ReceiveEvent((AtkEventType)37, 7, &evt, &data);
        }

        public void SetComment(string s)
        {
            var inp = Base->GetComponentNodeById(22);
            var tnode = (AtkComponentTextInput*)inp->Component;
            var txt = inp->Component->UldManager.SearchNodeById(16)->GetAsAtkTextNode();
            var san = Chat.Instance.SanitiseText(s);
            if(s != san) throw new ArgumentOutOfRangeException("String contains invalid characters!");
            if(s.Length > 192) throw new InvalidOperationException("String exceeds maximum length");
            if(s.Split("\n").Length > 2) throw new InvalidOperationException("String contains more than 2 lines");
            tnode->SetText(s);
            //txt->SetText(s);
            //Callback.Fire(Base, true, 15, s, Callback.ZeroAtkValue);
        }
    }
}
