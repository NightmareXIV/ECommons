using ECommons.StringHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMContentsBooty(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public string CoinsString => ReadString(2);
    public int    Coins       => DigitParsers.FirstNumber(CoinsString.Replace(",", ""));

    public string LootCoinsString => ReadString(4);
    public int    LootCoins       => DigitParsers.FirstNumber(LootCoinsString.Replace(",", ""));
    public bool   LootCoinsTaken  => ReadBool(5) ?? false;

    private const int LootOffset = 9;
    private const int LootEntrySize = 5;
    public List<LootChoice> LootChoices => Loop<LootChoice>(LootOffset, LootEntrySize, 4);

    public class LootChoice(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public readonly int lootIndex = (BeginOffset - LootOffset) / LootEntrySize;

        public uint Item => ReadUInt(0) ?? 0u;
        public bool Unk4 => ReadBool(2) ?? false;

        public bool Taken => ReadBool(129 + lootIndex - this.AtkReaderParams.BeginOffset) ?? false;
    }


    public List<ReaderXBMContentsItemShop.ItemEntry>        ItemEntries      => Loop<ReaderXBMContentsItemShop.ItemEntry>(27, ReaderXBMContentsItemShop.ITEM_ENTRY_SIZE, ReaderXBMContentsItemShop.ITEM_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.ItemEntry> ItemEntriesValid => ItemEntries.Where(ie => ie.Id > 0);

    public List<ReaderXBMContentsItemShop.GearEntry> OwnedEntries => Loop<ReaderXBMContentsItemShop.GearEntry>(78, ReaderXBMContentsItemShop.GEAR_ENTRY_SIZE, ReaderXBMContentsItemShop.GEAR_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.GearEntry> OwnedEntriesOwned => OwnedEntries.Where(oe => oe.Owned);
}