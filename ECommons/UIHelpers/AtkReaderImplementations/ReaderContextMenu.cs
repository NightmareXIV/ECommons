using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
#nullable disable

public unsafe class ReaderContextMenu(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public uint Count => ReadUInt(0) ?? 0;
    public List<ContextMenuEntry> Entries => Loop<ContextMenuEntry>(7, 1, (int)Count);

    public unsafe class ContextMenuEntry(nint Addon, int start) : AtkReader(Addon, start)
    {
        public string Name => ReadString(0);
        public SeString NameSeString => ReadSeString(0);
    }
}
