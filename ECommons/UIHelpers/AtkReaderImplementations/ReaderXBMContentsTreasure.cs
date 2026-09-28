using Dalamud.Game.Text.SeStringHandling;
using ECommons.StringHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMContentsTreasure(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public string CoinsString => ReadString(2);
    public int    Coins       => DigitParsers.FirstNumber(CoinsString.Replace(",", ""));

    private const int TreasureOffset = 3;
    private const int TreasureEntrySize = 5;

    public List<TreasureChoice> TreasureChoices => Loop<TreasureChoice>(TreasureOffset, TreasureEntrySize, (int)4);

    public class TreasureChoice(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public readonly int treasureIndex = (BeginOffset - TreasureOffset) / TreasureEntrySize;

        public bool Unk0 => ReadBool(0) ?? false;
        public uint Item => ReadUInt(3) ?? 0u;
        public bool Bought => ReadBool(126 + treasureIndex - this.AtkReaderParams.BeginOffset) ?? false;
    }


    public List<ReaderXBMContentsItemShop.ItemEntry>        ItemEntries      => Loop<ReaderXBMContentsItemShop.ItemEntry>(24, ReaderXBMContentsItemShop.ITEM_ENTRY_SIZE, ReaderXBMContentsItemShop.ITEM_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.ItemEntry> ItemEntriesValid => ItemEntries.Where(ie => ie.Id > 0);


    public List<ReaderXBMContentsItemShop.GearEntry> OwnedEntries => Loop<ReaderXBMContentsItemShop.GearEntry>(75, ReaderXBMContentsItemShop.GEAR_ENTRY_SIZE, ReaderXBMContentsItemShop.GEAR_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.GearEntry> OwnedEntriesOwned => OwnedEntries.Where(oe => oe.Owned);
}