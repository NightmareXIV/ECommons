using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;
using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class Request : AddonMasterBase<AddonRequest>
    {
        public Request(nint addon) : base(addon)
        {
        }

        public Request(void* addon) : base(addon) { }

        public AtkComponentButton* HandOverButton => Base->GetButtonNodeById(14);

        public void HandOver() => ClickButtonIfEnabled(HandOverButton);

        public bool IsHandOverEnabled => HandOverButton->IsEnabled;

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