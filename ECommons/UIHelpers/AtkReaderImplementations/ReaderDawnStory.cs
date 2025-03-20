using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderDawnStory(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public uint CurrentSelection => ReadUInt(21) ?? 0;
    public uint AvailableExpansions => ReadUInt(8) ?? 0;
    public uint CurrentExpansion => ReadUInt(423) ?? 0;
    public uint EntryCount => ReadUInt(22) ?? 0;
    public List<Entry> Entries => Loop<Entry>(23, 3, (int)EntryCount);
    public List<EntryName> EntryNames => Loop<EntryName>(263, 2, (int)EntryCount, true);
    public class Entry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint Unk0 => ReadUInt(0) ?? 0;
        public uint Callback => ReadUInt(1) ?? 0;
        public uint Status => ReadUInt(2) ?? 2;
    }

    public class EntryName(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public SeString Name => ReadSeString(0);
        public SeString Level => ReadSeString(1);
    }
}
