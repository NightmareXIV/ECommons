using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Client.UI;
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

        public void RepairAll() => ClickButtonIfEnabled(Addon->RepairAllButton);
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