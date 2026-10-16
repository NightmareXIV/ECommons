using ECommons.StringHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMContentsMainHUD(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public string CoinsString => ReadString(8);
    public int    Coins       => DigitParsers.FirstNumber(CoinsString.Replace(",", ""));

    private const int ItemOffset = 9;
    public int GetItemIndex(ReaderXBMContentsItemShop.ItemEntry item)
    {
        return item.GetIndex(ItemOffset);
    }
    public List<ReaderXBMContentsItemShop.ItemEntry>        ItemEntries      => Loop<ReaderXBMContentsItemShop.ItemEntry>(ItemOffset, ReaderXBMContentsItemShop.ITEM_ENTRY_SIZE, ReaderXBMContentsItemShop.ITEM_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.ItemEntry> ItemEntriesValid => ItemEntries.Where(ie => ie.Id > 0);


    public List<ReaderXBMContentsItemShop.GearEntry>        OwnedEntries      => Loop<ReaderXBMContentsItemShop.GearEntry>(60, ReaderXBMContentsItemShop.GEAR_ENTRY_SIZE, ReaderXBMContentsItemShop.GEAR_ENTRY_LENGTH);
    public IEnumerable<ReaderXBMContentsItemShop.GearEntry> OwnedEntriesOwned => OwnedEntries.Where(oe => oe.Owned);
}