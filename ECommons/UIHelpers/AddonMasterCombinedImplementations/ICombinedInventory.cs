using ECommons.UIHelpers.AddonMasterImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterCombinedImplementations;
public static unsafe partial class AddonMasterCombined
{
    public interface ICombinedInventory 
    {
        public void OpenArmoryChest();

        public static ICombinedInventory Get()
        {
            {
                if(GenericHelpers.TryGetAddonMaster<AddonMaster.Inventory>("Inventory", out var m) && m.IsAddonReady)
                {
                    return m;
                }
            }
            {
                if(GenericHelpers.TryGetAddonMaster<AddonMaster.InventoryLarge>("InventoryLarge", out var m) && m.IsAddonReady)
                {
                    return m;
                }
            }
            {
                if(GenericHelpers.TryGetAddonMaster<AddonMaster.InventoryExpansion>("InventoryExpansion", out var m) && m.IsAddonReady)
                {
                    return m;
                }
            }
            return null;
        }
    }
}