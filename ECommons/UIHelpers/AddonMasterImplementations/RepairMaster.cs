using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe class RepairMaster : AddonMasterBase<AddonRepair>
{
    public RepairMaster(nint addon) : base(addon)
    {
    }

    public RepairMaster(void* addon) : base(addon) { }

    public void RepairAll()
    {
        var btn = Addon->RepairAllButton;
        if (btn->IsEnabled)
        {
            btn->ClickAddonButton(Base);
        }
    }
}

