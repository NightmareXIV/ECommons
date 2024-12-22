using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class Talk : AddonMasterBase<AddonTalk>
    {
        public Talk(nint addon) : base(addon)
        {
        }

        public Talk(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Subtitle box";

        public void Click()
        {
            var evt = stackalloc AtkEvent[1]
            {
                CreateAtkEvent(132),
            };
            var data = stackalloc AtkEventData[1];
            for(int i = 0; i < sizeof(AtkEventData); i++)
            {
                ((byte*)data)[i] = 0;
            }
            Base->ReceiveEvent(AtkEventType.MouseDown, 0, evt, data);
            Base->ReceiveEvent(AtkEventType.MouseClick, 0, evt, data);
            Base->ReceiveEvent(AtkEventType.MouseUp, 0, evt, data);
        }
    }
}