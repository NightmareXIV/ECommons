using Dalamud.Bindings.ImGui;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using Callback = ECommons.Automation.Callback;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class ShopExchangeCurrency : AddonMasterBase<AtkUnitBase>
    {
        public ShopExchangeCurrency(nint addon) : base(addon) { }
        public ShopExchangeCurrency(void* addon) : base(addon) { }

        public uint CurrencyAmount => Addon->AtkValues[84].UInt;
        public uint NumEntries => Addon->AtkValues[4].UInt;
        public uint CurrencyId
        {
            get
            {
                var iconId = Addon->AtkValues[85].UInt;
                var row = Svc.Data.GetExcelSheet<Item>().Where(x => x.Icon == iconId).FirstOrDefault().RowId;
                if (row != 0)
                {
                    return row;
                }
                else
                {
                    return 0;
                }
            }
        }
        // 1064 - Start of ItemIds
        // 454 - Start of Shop Price

        public class ShopItemInfo(ShopExchangeCurrency master)
        {
            public uint ItemId;
            public uint Index;
            public uint CostAmount;
            public void Select(int amount = 1)
            {
                Callback.Fire(master.Base, true, 0, Index, amount);
            }
        }

        /// <summary>
        /// This exist as it is because "ShopExchangeCurrency" covers...  alot of different shop types that exist. <br></br>
        /// This just covers the basic "Item -> Gil/Currency Exchange", since ones where you need to exchange items are coded differently
        /// </summary>
        public ShopItemInfo[] BasicShopItems
        {
            get
            {
                var ret = new List<ShopItemInfo>();
                for (int i = 0; i < NumEntries; i++)
                {
                    var itemId = Addon->AtkValues[1064 + (i * 1)].UInt;

                    if(itemId == 0)
                        continue;
                    else
                    {
                        var costAmount = Addon->AtkValues[454 + (i * 1)].UInt;
                        var index = Addon->AtkValues[1308 + (i * 1)].UInt;
                        var newEntry = new ShopItemInfo(this)
                        {
                            ItemId = itemId,
                            Index = index,
                            CostAmount = costAmount
                        };
                        ret.Add(newEntry);
                    }
                }
                return [.. ret];
            }
        }


        public override string AddonDescription { get; } = "Item Exchange Window";
    }
}
