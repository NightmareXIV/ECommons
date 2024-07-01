using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;
using System;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class Repair : AddonMasterBase<AddonRepair>
    {
        public Repair(nint addon) : base(addon)
        {
        }

        public Repair(void* addon) : base(addon) { }

        public void RepairAll()
        {
            var btn = Addon->RepairAllButton;
            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }
    }
}

[Obsolete("Please use AddonMaster.Repair")]
public unsafe class RepairMaster : AddonMaster.Repair
{
    public RepairMaster(nint addon) : base(addon)
    {
    }

    public RepairMaster(void* addon) : base(addon)
    {
    }
}