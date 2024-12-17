using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderMiragePrismPrismSetConvert(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public uint Unk00 => ReadUInt(0) ?? 0;
    public uint GlamourPrismsHeld => ReadUInt(1) ?? 0;
    public uint Unk02 => ReadUInt(2) ?? 0;
    public uint Unk03 => ReadUInt(3) ?? 0;
    public uint Unk04 => ReadUInt(4) ?? 0;
    public uint OutfitIconId => ReadUInt(5) ?? 0;
    /// <remarks>
    /// Also the amount of glamour prisms required
    /// </remarks>
    public uint ItemCount => ReadUInt(15) ?? 0;
    public List<Item> Items => Loop<Item>(16, 7, (int)ItemCount);

    public unsafe class Item(nint Addon, int start) : AtkReader(Addon, start)
    {
        public uint ItemId => ReadUInt(0) ?? 0;
        public uint ItemIconId => ReadUInt(1) ?? 0;
        public uint Unk03 => ReadUInt(2) ?? 0;
        public uint Unk04 => ReadUInt(3) ?? 0;
        public uint InventoryType => ReadUInt(4) ?? 0; // 9999 if the item hasn't been filled
        public uint InventorySlot => ReadUInt(5) ?? 0; // 0 if item hasn't been filled
        public uint Unk07 => ReadUInt(6) ?? 0; // 2 if item hasn't been filled
    }
}
