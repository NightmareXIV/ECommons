using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using Callback = ECommons.Automation.Callback;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class Shop : AddonMasterBase<AtkUnitBase>
    {
        public Shop(nint addon) : base(addon) { }
        public Shop(void* addon) : base(addon) { }
        public uint NumEntries => Addon->AtkValues[2].UInt;
        public class ShopItemInfo(Shop master, int index)
        {
            public uint ItemId;
            public uint CostAmount;
            public void Select(int amount = 1)
            {
                Callback.Fire(master.Base, true, 0, index, amount);
            }
        }
        public class CostInfo
        {
            public uint itemId;
            public uint cost;
        }

        /// <summary>
        /// This exist as it is because "ShopExchangeCurrency" covers...  alot of different shop types that exist. <br></br>
        /// This just covers the basic "Item -> Gil/Currency Exchange", since ones where you need to exchange items are coded differently
        /// </summary>
        public ShopItemInfo[] ShopItems
        {
            get
            {
                var ret = new List<ShopItemInfo>();
                for(int i = 0; i < NumEntries; i++)
                {
                    var itemId = Addon->AtkValues[441 + (i * 1)].UInt;

                    if(itemId == 0)
                        continue;
                    else
                    {
                        var costAmount = Addon->AtkValues[75 + (i * 1)].UInt;
                        var newEntry = new ShopItemInfo(this, i)
                        {
                            ItemId = itemId,
                            CostAmount = costAmount
                        };
                        ret.Add(newEntry);
                    }
                }
                return [.. ret];
            }
        }

        public override string AddonDescription { get; } = "Basic Gil Shop Window";
    }
}
