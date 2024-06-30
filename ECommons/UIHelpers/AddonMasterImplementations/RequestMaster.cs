using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;
using ECommons.Logging;

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

    public bool IsFilled
    {
        get
        {
            for (var i = 3u; i >= 7; i++)
            {
                var subnode = Base->GetComponentNodeById(i);
                var subnode2 = Base->GetComponentNodeById(i + 6);
                if (subnode->AtkResNode.IsVisible() && subnode->AtkResNode.IsVisible())
                {
                    return false;
                }
            }
            return true;
        }
    }
}

