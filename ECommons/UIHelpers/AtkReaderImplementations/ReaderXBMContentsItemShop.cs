using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMContentsItemShop(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{

    public string CoinsString => ReadString(1);
    public uint   Coins       => uint.TryParse(CoinsString.Replace(",", ""), out var value) ? value : 0u;


    public uint StockCount => ReadUInt(2) ?? 0u;


    private const int StockOffset    = 3;
    private const int StockEntrySize = 5;

    public List<StockEntry> StockEntries => Loop<StockEntry>(StockOffset, StockEntrySize, (int)StockCount);

    public class StockEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public readonly int purchaseIndex = (BeginOffset - StockOffset) / StockEntrySize;

        public bool Listed => ReadBool(0) ?? false;

        public uint     Item        => ReadUInt(1) ?? 0u;
        public SeString PriceString => ReadSeString(2);
        public uint     Price
        {
            get
            {
                var text = PriceString.GetText().Replace(",", "").Trim();
                var ind  = text.IndexOf('(');

                return uint.TryParse(ind >= 0 ? text[..ind] : text, out var rankString) ? rankString : 0;
            }
        }

        public bool Discounted => ReadBool(3) ?? false;
        public bool Bought     => ReadBool(4) ?? false;
    }

    public const int ITEM_ENTRY_SIZE   = 5;
    public const int ITEM_ENTRY_LENGTH = 10;

    public       List<ItemEntry>        ItemEntries      => Loop<ItemEntry>(154, ITEM_ENTRY_SIZE, ITEM_ENTRY_LENGTH);
    public       IEnumerable<ItemEntry> ItemEntriesValid => ItemEntries.Where(ie => ie.Id > 0);

    public class ItemEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public bool   Unk0     => ReadBool(0) ?? false;

        /// <summary>
        /// In Shops: Sellable. In Main: Usable
        /// </summary>
        public bool   Available => ReadBool(1) ?? false;
        public uint   IconId   => ReadUInt(2) ?? 0;
        public uint   Id       => ReadUInt(3) ?? 0;
        public string Name     => ReadString(4);
    }

    public const int GEAR_ENTRY_SIZE   = 5;
    public const int GEAR_ENTRY_LENGTH = 10;

    public List<GearEntry>        OwnedEntries      => Loop<GearEntry>(205, GEAR_ENTRY_SIZE, GEAR_ENTRY_LENGTH);
    public IEnumerable<GearEntry> OwnedEntriesOwned => OwnedEntries.Where(oe => oe.Owned);

    public class GearEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public bool   Owned  => ReadBool(0) ?? false;
        public uint   IconId => ReadUInt(2) ?? 0;
        public uint   Id     => ReadUInt(3) ?? 0;
        public string Name   => ReadString(4);
    }
}