using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using Callback = ECommons.Automation.Callback;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class InclusionShop : AddonMasterBase<AtkUnitBase>
    {
        public InclusionShop(nint addon) : base(addon) { }
        public InclusionShop(void* addon) : base(addon) { }
        public uint CurrencyAmount => Addon->AtkValues[297].UInt;
        public uint NumEntries => Addon->AtkValues[298].UInt;

        public class ShopItemInfo(InclusionShop master, int index)
        {
            public uint ItemId;
            public uint CurrencyId;
            public uint Cost;
            public void Select(int amount = 1)
            {
                Callback.Fire(master.Base, true, 14, index, amount);
            }
        }

        public ShopItemInfo[] ShopItems
        {
            get
            {
                var ret = new List<ShopItemInfo>();
                for(int i = 0; i < NumEntries; i++)
                {
                    var itemId = Addon->AtkValues[300 + (i * 18)].UInt;

                    if (itemId == 0)
                        continue;
                    else
                    {
                        var costItemId = Addon->AtkValues[305 + (i * 18)].UInt;
                        var costAmount = Addon->AtkValues[311 + (i * 18)].UInt;

                        var itemEntry = new ShopItemInfo(this, i)
                        {
                            ItemId = itemId,
                            CurrencyId = costItemId,
                            Cost = costAmount,
                        };
                        ret.Add(itemEntry);
                    }
                }
                return [.. ret];
            }
        }

        public override string AddonDescription { get; } = "Crafter/Gathering Script Shop Window";
    }
}
