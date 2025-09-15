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
    public class Inventory : AddonMasterBase<AddonInventory>, AddonMasterCombined.ICombinedInventory
    {
        public Inventory(nint addon) : base(addon)
        {
        }

        public Inventory(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Inventory window";

        public void OpenArmoryChest()
        {
            this.ClickButtonIfEnabled(this.Addon->AtkUnitBase.GetComponentButtonById(17));
        }
    }
}