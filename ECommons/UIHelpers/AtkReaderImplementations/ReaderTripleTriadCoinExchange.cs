using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderTripleTriadCoinExchange(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public uint EntryCount => ReadUInt(1) ?? 0;
    public List<CardEntry> Entries => Loop<CardEntry>(4, 1, (int)EntryCount);
    public class CardEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint Unk0 => ReadUInt(0) ?? 0;
        public string Name => ReadString(40);
        public uint Value => ReadUInt(80) ?? 0;
        public uint Count => ReadUInt(120) ?? 0;
        public uint Id => ReadUInt(160) ?? 0;
        public bool InDeck => ReadBool(200) ?? false;
    }
}