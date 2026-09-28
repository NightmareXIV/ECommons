using Dalamud.Game.Text.SeStringHandling;
using ECommons.StringHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMMonsterNotebook(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public const int EntryCountPerPage = 25;

    public uint               PageCount          => ReadUInt(9)  ?? 0;
    public uint               CurrentPage        => ReadUInt(10) ?? 0;
    public List<MonsterEntry> CurrentPageEntries => Loop<MonsterEntry>(24, 8, EntryCountPerPage);

    public class MonsterEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint     Number       => ReadUInt(0) ?? 0u;
        public bool     Unk1         => ReadBool(1) ?? false;
        public bool     Caught       => ReadBool(2) ?? false;
        public bool     Unk3         => ReadBool(3) ?? false;
        public uint     Unk4         => ReadUInt(4) ?? 0u;
        public SeString NumberString => ReadSeString(5);
        public bool     Unk6         => ReadBool(6) ?? false;
        public uint     Unk7         => ReadUInt(7) ?? 0;
    }

    public uint CurrentNumber  => ReadUInt(5) ?? 0;
    public uint SelectedNumber => ReadUInt(6) ?? 0;
    public bool Filtered => ReadBool(8) ?? false;

    public bool Selected => ReadBool(255) ?? false;

    public bool     CurrentRankCapped     => ReadBool(237) ?? false;
    public SeString CurrentName           => ReadSeString(230);
    public SeString CurrentClassification => ReadSeString(239);

    public SeString CurrentRankString => ReadSeString(258);
    public int      CurrentRank       => int.TryParse(CurrentRankString.GetText().Trim(), out var rank) ? rank : 0;
    public uint     CurrentXP         => ReadUInt(261) ?? 0;
    public SeString CurrentHP => ReadSeString(259);
    public int CurrentMaxHP
    {
        get
        {
            var text = CurrentHP.GetText();
            var ind  = text.IndexOf('/');

            return DigitParsers.Digits(text[(ind + 1)..]);
        }
    }
    public SeString CurrentStrengthString => ReadSeString(265);
    public int      CurrentStrength       => int.TryParse(CurrentStrengthString.GetText().Trim(), out var i) ? i : 0;

    public SeString CurrentPhysResistanceString => ReadSeString(267);
    public int CurrentPhysResistance => int.TryParse(CurrentPhysResistanceString.GetText().Trim(), out var i) ? i : 0;

    public SeString CurrentConstitutionString => ReadSeString(269);
    public int CurrentConstitution => int.TryParse(CurrentConstitutionString.GetText().Trim(), out var i) ? i : 0;

    public SeString CurrentIntelligenceString => ReadSeString(271);
    public int CurrentIntelligence => int.TryParse(CurrentIntelligenceString.GetText().Trim(), out var i) ? i : 0;

    public SeString CurrentMagicResistanceString => ReadSeString(273);
    public int CurrentMagicResistance => int.TryParse(CurrentMagicResistanceString.GetText().Trim(), out var i) ? i : 0;
}