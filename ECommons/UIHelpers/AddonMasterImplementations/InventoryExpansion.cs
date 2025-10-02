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
    public class InventoryExpansion : AddonMasterBase<AddonInventoryExpansion>, AddonMasterCombined.ICombinedInventory
    {
        public InventoryExpansion(nint addon) : base(addon)
        {
        }

        public InventoryExpansion(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Quadruple Inventory window";

        public void OpenArmoryChest()
        {
            this.ClickButtonIfEnabled(this.Addon->AtkUnitBase.GetComponentButtonById(140));
        }
    }
}