using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class MJIDisposeShop : AddonMasterBase<AtkUnitBase>
    {
        public MJIDisposeShop(nint addon) : base(addon) { }
        public MJIDisposeShop(void* addon) : base(addon) { }
        public override string AddonDescription => "Island Sanc Export Shop";

        public uint NumEntries => Addon->AtkValues[8].UInt;

        public class ExportShopInfo(MJIDisposeShop master, int index)
        {
            public uint IconId;
            public string ItemName;
            public uint Inventory;
            public uint Value;
            public uint SellIconId;
            public uint Allocated;

            public void Select()
            {
                Callback.Fire(master.Base, true, 12, index);
            }
        }

        public ExportShopInfo[] ExportItems
        {
            get
            {
                var ret = new List<ExportShopInfo>();
                for (int i = 0; i < NumEntries; i++)
                {
                    var itemIconId = Addon->AtkValues[10 + (i * 8)].UInt;
                    if(itemIconId == 0)
                        continue;
                    else
                    {
                        var itemName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[11 + (i * 8)].String.Value).GetText();
                        var inventory = Addon->AtkValues[12 + (i * 8)].UInt;
                        var value = Addon->AtkValues[14 + (i * 8)].UInt;
                        var sellIconId = Addon->AtkValues[15 + (i * 8)].UInt;
                        var allocated = Addon->AtkValues[16 + (i * 8)].UInt;

                        var newEntry = new ExportShopInfo(this, i)
                        {
                            IconId = itemIconId,
                            ItemName = itemName,
                            Inventory = inventory,
                            Value = value,
                            SellIconId = sellIconId,
                            Allocated = allocated
                        };
                        ret.Add(newEntry);
                    }
                }

                return [.. ret];
            }
        }
    }
}
