using System.Reflection;

namespace ECommons.ExcelServices;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public enum TerritoryIntendedUse : uint
{
    Unknown = 0,
    AllianceRaid,
    Dungeon,
    House,
    Inn,
    MainCity,
    OpenArea,
    Prison,
    Raid,
    OldRaid,
    Residential,
    Trial,
    Variant
}
