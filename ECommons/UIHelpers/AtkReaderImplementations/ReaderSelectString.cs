using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
#nullable disable

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderSelectString(AtkUnitBase* a) : AtkReader(a)
{
    public string Description => ReadString(2);
    public int NumEntries => ReadInt(3) ?? 0;
    public List<Entry> Entries => Loop<Entry>(7, 1, NumEntries);

    public unsafe class Entry(nint a, int s) : AtkReader(a, s)
    {
        public string Text => ReadString(0);
    }
}
