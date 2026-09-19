using ECommons.Automation.UIInput;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe partial class AddonMaster
{
    public class FreeCompanyChest : AddonMasterBase
    {
        public FreeCompanyChest(nint addon) : base(addon)
        {
        }

        public FreeCompanyChest(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Free Company Chest";

        public List<Button> ItemsButtons
        {
            get
            {
                var ret = new List<Button>();
                for(var i = 0; i < 5; i++)
                {
                    var button = new Button(this)
                    {
                        Handle = Addon->GetComponentNodeById((uint)(10 + i))->GetAsAtkComponentRadioButton()
                    };
                    if(button.Accessible) ret.Add(button);
                }
                return ret;
            }
        }

        public class Button(FreeCompanyChest AddonMaster)
        {
            public AtkComponentRadioButton* Handle;
            public bool Accessible => Handle->AtkResNode->IsVisible();
            public void Select()
            {
                AddonMaster.ClickButtonIfEnabled(Handle);
            }
        }
    }
}