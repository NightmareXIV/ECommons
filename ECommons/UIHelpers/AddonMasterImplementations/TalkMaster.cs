using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe class TalkMaster : AddonMasterBase<AddonTalk>
{
    public TalkMaster(nint addon) : base(addon)
    {
    }

    public TalkMaster(void* addon) : base(addon)
    {
    }

    public bool IsVisible => Base->IsVisible;

    public void Click()
    {
        var evt = stackalloc AtkEvent[1]
        {
                new()
                {
                    Listener = (AtkEventListener*)Base,
                    Flags = 132,
                    Target = &AtkStage.Instance()->AtkEventTarget
                }
            };
        var data = stackalloc AtkEventData[1];
        Base->ReceiveEvent(AtkEventType.MouseClick, 0, evt, data);
    }
}
