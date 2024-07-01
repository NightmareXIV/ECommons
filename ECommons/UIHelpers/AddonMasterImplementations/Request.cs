using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;
using System;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class Request : AddonMasterBase<AddonRequest>
    {
        public Request(nint addon) : base(addon)
        {
        }

        public Request(void* addon) : base(addon) { }

        public void HandOver()
        {
            var btn = Addon->HandOverButton;
            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }
    }
}

[Obsolete("Please use AddonMaster.Request")]
public unsafe class RequestMaster : AddonMaster.Request
{
    public RequestMaster(nint addon) : base(addon)
    {
    }

    public RequestMaster(void* addon) : base(addon)
    {
    }
}