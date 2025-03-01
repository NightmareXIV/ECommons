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
    public uint CurrentSelection => this.ReadUInt(21) ?? 0;
    public uint AvailableExpansions => this.ReadUInt(8) ?? 0;
    public uint CurrentExpansion => this.ReadUInt(423) ?? 0;
    public uint EntryCount => this.ReadUInt(22) ?? 0;
    public List<Entry> Entries => this.Loop<Entry>(23, 3, (int)this.EntryCount);
    public List<EntryName> EntryNames => this.Loop<EntryName>(263, 2, (int)this.EntryCount, true);
    public class Entry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint Unk0 => this.ReadUInt(0) ?? 0;
        public uint Callback => this.ReadUInt(1) ?? 0;
        public uint Status => this.ReadUInt(2) ?? 2;
    }

    public class EntryName(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public SeString Name => this.ReadSeString(0);
        public SeString Level => this.ReadSeString(1);
    }
}
