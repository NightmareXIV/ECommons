using ECommons.UIHelpers.AddonMasterCombinedImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe partial class AddonMaster
{
    public class InventoryLarge : AddonMasterBase<AddonInventoryLarge>, AddonMasterCombined.ICombinedInventory
    {
        public InventoryLarge(nint addon) : base(addon)
        {
        }

        public InventoryLarge(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Double Inventory window";

        public void OpenArmoryChest()
        {
            this.ClickButtonIfEnabled(this.Addon->AtkUnitBase.GetComponentButtonById(71));
        }
    }
}