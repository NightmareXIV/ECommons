using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderLetterHistory(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public uint LetterCount => ReadUInt(0) ?? 0;
    public List<Letter> Items => Loop<Letter>(2, 20, 20);

    public unsafe class Letter(nint Addon, int start) : AtkReader(Addon, start)
    {
        public string Recipient => ReadString(0);
        public uint Unk20 => ReadUInt(20) ?? 0;
        public bool CanReply => ReadBool(40) ?? false; // I don't know what determines this bool. Being able to reply is the only consequence I can see
        public string SentDate => ReadString(60);
        public uint NumItems => ReadUInt(80) ?? 0;
        public List<Item> Items => Loop<Item>(102, 5, 20);
        public string Gil => ReadString(300);
        
    }

    public unsafe class Item(nint Addon, int start) : AtkReader(Addon, start)
    {
        public string ItemNameAndQuantity => ReadString(0);
        public uint ItemQuantity => ReadUInt(100) ?? 0;
    }
}
