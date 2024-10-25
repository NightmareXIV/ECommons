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
                    ret[i] = new(this, i);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Free Company credit shop window";

        public readonly struct Item
        {
            public int Index { get; init; }
            public string ItemName { get; init; }
            public uint ItemId { get; init; }
            public int IconId { get; init; }
            public uint Rank { get; init; }
            public int QuantityInInventory { get; init; }
            public int MaxPurchaseSize { get; init; }
            public uint Price { get; init; }
            private readonly FreeCompanyCreditShop Am;

            public Item(FreeCompanyCreditShop am, int index)
            {
                Am = am;
                Index = index;
                ItemName = MemoryHelper.ReadSeStringNullTerminated((nint)Am.Addon->AtkValues[10 + index].String).ExtractText();
                ItemId = Am.Addon->AtkValues[30 + index].UInt;
                IconId = Am.Addon->AtkValues[50 + index].Int;
                Rank = Am.Addon->AtkValues[70 + index].UInt;
                QuantityInInventory = (int)Am.Addon->AtkValues[90 + index].UInt;
                MaxPurchaseSize = Am.Addon->AtkValues[110 + index].Int;
                Price = Am.Addon->AtkValues[130 + index].UInt; // for a single unit
            }

            public readonly void Buy(int quantity)
            {
                if (quantity <= MaxPurchaseSize)
                {
                    if (quantity * Price <= Am.CompanyCredits)
                        Callback.Fire(Am.Addon, true, 0, Index, quantity);
                    else
                        PluginLog.LogError($"Unable to purchase {quantity}x of {ItemId}. Insufficient company credits (requires {quantity * Price}, have {Am.CompanyCredits})");
                }
                else
                    PluginLog.LogError($"Unable to purchase {quantity}x of {ItemId}. Quantity exceeds max purchase size of {MaxPurchaseSize}");
            }

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
