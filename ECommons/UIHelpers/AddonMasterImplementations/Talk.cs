﻿using FFXIVClientStructs.FFXIV.Client.UI;
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
}