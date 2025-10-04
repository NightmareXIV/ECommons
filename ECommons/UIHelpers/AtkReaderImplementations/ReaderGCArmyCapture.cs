using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderGCArmyCapture(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public SeString PlayerCharName => ReadSeString(5);
    public string PlayerCharIlvl => ReadString(4);
    public uint PlayerCharLvl => ReadUInt(3) ?? 0;
    public uint EntryCount => ReadUInt(7) ?? 0;
    public List<DungeonInfo> Entries => Loop<DungeonInfo>(9, 6, (int)EntryCount);

    public class DungeonInfo(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public bool Unk0 => ReadBool(0) ?? false;
        public bool Completed => ReadBool(1) ?? false;
        public uint Unk2 => ReadUInt(2) ?? 0;
        public SeString Name => ReadSeString(3);
        public string Level => ReadString(4);
        public bool Synced => ReadBool(5) ?? false;
    }
}