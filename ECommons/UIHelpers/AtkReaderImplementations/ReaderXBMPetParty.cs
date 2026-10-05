using Dalamud.Game.Text.SeStringHandling;
using ECommons.StringHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;

public unsafe class ReaderXBMPetParty(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
{
    public uint               EntryCount  => ReadUInt(5) ?? 0;
    public List<MonsterEntry> TeamEntries => Loop<MonsterEntry>(6, 77, (int)EntryCount);


    public uint TeamSize => ReadUInt(1162) ?? 0;

    public FedEntry FeedItem => new((nint)UnitBase, 1186);

    public class MonsterEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public readonly int index = (BeginOffset - 6) / 77;

        public SeString RankString => ReadSeString(0);

        public int Rank => DigitParsers.FirstNumber(RankString.GetText());

        public uint IconId   => ReadUInt(1) ?? 0;
        public bool Disabled => ReadBool(2) ?? false;

        public SeString Name  => ReadSeString(3);
        public uint     HP    => ReadUInt(5) ?? 0;
        public uint     MaxHP => ReadUInt(6) ?? 0;

        public SeString StrengthString => ReadSeString(19);
        public int      Strength       => int.TryParse(StrengthString.GetText().Trim(), out var i) ? i : 0;

        public SeString PhysResistanceString => ReadSeString(21);
        public int      PhysResistance       => int.TryParse(PhysResistanceString.GetText().Trim(), out var i) ? i : 0;

        public SeString ConstitutionString => ReadSeString(23);
        public int      Constitution       => int.TryParse(ConstitutionString.GetText().Trim(), out var i) ? i : 0;

        public SeString IntelligenceString => ReadSeString(25);
        public int      Intelligence       => int.TryParse(IntelligenceString.GetText().Trim(), out var i) ? i : 0;

        public SeString MagicResistanceString => ReadSeString(27);
        public int      MagicResistance       => int.TryParse(MagicResistanceString.GetText().Trim(), out var i) ? i : 0;


        public bool Slow                 => ReadBool(47) ?? false;
        public bool Petrification_Freeze => ReadBool(48) ?? false;
        public bool Paralysis            => ReadBool(49) ?? false;
        public bool Interruption         => ReadBool(50) ?? false;
        public bool Blind                => ReadBool(51) ?? false;
        public bool Stun                 => ReadBool(52) ?? false;
        public bool Sleep                => ReadBool(53) ?? false;
        public bool Bind                 => ReadBool(54) ?? false;
        public bool Heavy                => ReadBool(55) ?? false;
        public bool FlatDamage_Death     => ReadBool(56) ?? false;
        public bool Poison               => ReadBool(57) ?? false;

        public uint           FedCurrent     => ReadUInt(72) ?? 0u;
        public uint           FedMax         => ReadUInt(73) ?? 0u;

        /// <summary>
        /// 0-2 Selected horns. 3 means not selected
        /// </summary>
        public uint           SelectionIndex => ReadUInt(74) ?? 0;
        public List<FedEntry> FedItems       => Loop<FedEntry>(BeginOffset + 13 - 6, 2, (int)FedCurrent);
        public uint           Number         => ReadUInt(76)     ?? 0u;
    }

    public class FedEntry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
    {
        public uint IconId => ReadUInt(0) ?? 0;
        public uint ItemId => ReadUInt(1) ?? 0;
    }
}