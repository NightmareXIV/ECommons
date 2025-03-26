using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderLetterViewer(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public string SenderName => ReadString(0);
    public List<Item> Items => Loop<Item>(1, 1, 5);

    public unsafe class Item(nint Addon, int start) : AtkReader(Addon, start)
    {
        public uint ItemIconId => ReadUInt(0) ?? 0; // -1 if no item
        public uint ItemId => ReadUInt(30) ?? 0;
        public string ItemName => ReadString(35);
    }
}
