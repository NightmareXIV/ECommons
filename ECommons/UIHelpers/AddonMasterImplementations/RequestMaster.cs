using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe class RequestMaster : AddonMasterBase<AddonRequest>
{
    public RequestMaster(nint addon) : base(addon)
    {
    }

    public RequestMaster(void* addon) : base(addon) { }

    public void HandOver()
    {
        var btn = Addon->HandOverButton;
        if (btn->IsEnabled)
        {
            btn->ClickAddonButton(Base);
        }
    }
}

