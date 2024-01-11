using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.Implementations;
#nullable disable

public unsafe class ReaderContextMenu(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public uint Count => ReadUInt(0) ?? 0;
    public List<ContextMenuEntry> Entries => Loop<ContextMenuEntry>(7, 1, (int)Count);

    public unsafe class ContextMenuEntry(nint Addon, int start) : AtkReader(Addon, start)
    {
        public string Name => ReadString(0);
    }
}
