using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMResult(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public uint Performance      => ReadUInt(2)  ?? 0;
    public uint MovesMadeScore   => ReadUInt(4)  ?? 0;
    public uint EnemyScore       => ReadUInt(6)  ?? 0;
    public uint EliteScore       => ReadUInt(8)  ?? 0;
    public uint BossScore        => ReadUInt(10) ?? 0;
    public uint RemainingHP      => ReadUInt(11) ?? 0;
    public uint RemainingHPScore => ReadUInt(12) ?? 0;

    public uint                  BonusBoostScore   => ReadUInt(13) ?? 0;
    public uint                  BonusBoostCount   => ReadUInt(14) ?? 0;
    public List<BonusBoostEntry> BonusBoostEntries => Loop<BonusBoostEntry>(15, 1, (int)BonusBoostCount);
    public class BonusBoostEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public SeString Name  => ReadSeString(0);
        public uint     Score => ReadUInt(16) ?? 0;
    }

    public uint            LootCount   => ReadUInt(47) ?? 0;
    public List<LootEntry> LootEntries => Loop<LootEntry>(48, 1, (int)LootCount);
    public class LootEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint     ItemId => ReadUInt(0) ?? 0;
        public uint     IconId => ReadUInt(5) ?? 0;
        public SeString Name   => ReadSeString(10);
        public uint     Count  => ReadUInt(15) ?? 0;
    }

    public uint TotalScore => ReadUInt(68) ?? 0;
    public SeString Rank => ReadSeString(69);


    public uint BeastCount => ReadUInt(72) ?? 0;

    private const int              BeastOffset = 73;
    public        List<BeastEntry> BeastEntries => Loop<BeastEntry>(BeastOffset, 1, (int)BeastCount);

    public class BeastEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        private const int Offset = 16;

        public int  IconId => ReadInt(0) ?? 0;
        public uint Number => (uint)(IconId - 242_000);

        public uint PrevXP => ReadUInt(1 * Offset) ?? 0;
        public uint NewXP  => ReadUInt(2 * Offset) ?? 0;

        public uint PrevRank => ReadUInt(3 * Offset) ?? 0;
        public uint NewRank  => ReadUInt(4 * Offset) ?? 0;
    }
}