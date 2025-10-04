using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderGCArmyMemberList(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public uint EntryCount => ReadUInt(4) ?? 0;
    public List<MemberInfo> Entries => Loop<MemberInfo>(4, 15, (int)EntryCount);

    public class MemberInfo(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint Unk0 => ReadUInt(0) ?? 0;
        public int SelectionMask => ReadInt(1) ?? 0; // Bitmask: 0 is always set? 1 and 4 are set when selected. Sometimes 14 is set
        public bool Selected => Bitmask.IsBitSet((short)SelectionMask, 1);
        public string Name => ReadString(2);
        public string Class => ReadString(3);
        public string PortraitPath => ReadString(4);
        public int ClassId => ReadInt(5) ?? 0; // See SquadronClassType
        public SquadronClassType ClassType => (SquadronClassType)ClassId;
        public int Level => ReadInt(6) ?? 0;
        public uint Unk3 => ReadUInt(7) ?? 0;
        public int Unk4 => ReadInt(8) ?? 0;
        public int Physical => ReadInt(9) ?? 0;
        public int Mental => ReadInt(10) ?? 0;
        public int Tactical => ReadInt(11) ?? 0;
        public string Chemistry => ReadString(12);
        public string Tactics => ReadString(14);
    }

    public enum SquadronClassType : int
    {
        Gladiator = 0,
        Pugilist = 1,
        Marauder = 2,
        Lancer = 3,
        Archer = 4,
        Conjurer = 5,
        Thaumaturge = 6,
        Arcanist = 7,
        Rogue = 8,
    }
}