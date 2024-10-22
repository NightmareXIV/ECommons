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

        public override string AddonDescription { get; } = "Repair window";

        public void RepairAll() => ClickButtonIfEnabled(Addon->RepairAllButton);
    }
}
