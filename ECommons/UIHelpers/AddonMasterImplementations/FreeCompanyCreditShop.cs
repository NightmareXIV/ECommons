using Dalamud.Memory;
using ECommons.Automation;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class FreeCompanyCreditShop : AddonMasterBase<AtkUnitBase>
    {
        public FreeCompanyCreditShop(nint addon) : base(addon) { }
        public FreeCompanyCreditShop(void* addon) : base(addon) { }

        public uint FreeCompanyRank => Addon->AtkValues[0].UInt;
        public bool Unk01 => Addon->AtkValues[1].Bool;
        public uint CompanyCredits => Addon->AtkValues[3].UInt;
        public bool Unk05 => Addon->AtkValues[5].Bool;
        public uint ItemCount => Addon->AtkValues[9].UInt;

        public Item[] Items
        {
            get
            {
                var ret = new Item[ItemCount];
                for (var i = 0; i < ret.Length; i++)
                    ret[i] = new(Addon, i);
                return ret;
            }
        }

        public struct Item
        {
            public AtkUnitBase* Addon;
            public int Index { get; init; }
            public string ItemName { get; init; }
            public uint ItemId { get; init; }
            public int IconId { get; init; }
            public uint Rank { get; init; }
            public int QuantityInInventory { get; init; }
            public int MaxPurchaseSize { get; init; }
            public uint Price { get; init; }

            public Item(AtkUnitBase* addon, int index)
            {
                Addon = addon;
                Index = index;
                ItemName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[10 + index].String).ExtractText();
                ItemId = Addon->AtkValues[30 + index].UInt;
                IconId = Addon->AtkValues[50 + index].Int;
                Rank = Addon->AtkValues[70 + index].UInt;
                QuantityInInventory = (int)Addon->AtkValues[90 + index].UInt;
                MaxPurchaseSize = Addon->AtkValues[110 + index].Int; // not certain this is what the value is for
                Price = Addon->AtkValues[130 + index].UInt; // for a single unit
            }

            public readonly void Buy(int quantity) => Callback.Fire(Addon, true, 0, Index, quantity);

            public override readonly string? ToString() => $"{nameof(AddonMaster)}.{nameof(FreeCompanyCreditShop)}.{nameof(Item)} [{nameof(ItemId)}={ItemId} {nameof(ItemName)}=\"{ItemName}\", {nameof(Index)}={Index}]";
        }

        public void Buy(uint itemId, int quantity)
        {
            if (Items.TryGetFirst(x => x.ItemId == itemId, out var item))
                item.Buy(quantity);
            else
                PluginLog.LogError($"Item id \"{itemId}\" not found in {nameof(FreeCompanyCreditShop)}.{nameof(Items)}");
        }
    }
}
