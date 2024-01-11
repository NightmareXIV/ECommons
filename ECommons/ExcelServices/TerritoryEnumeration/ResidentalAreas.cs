using System.Collections.Generic;
using System.Reflection;
#nullable disable

namespace ECommons.ExcelServices.TerritoryEnumeration;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public static class ResidentalAreas
{
    public const ushort Mist = 339;
    public const ushort The_Lavender_Beds = 340;
    public const ushort The_Goblet = 341;
    public const ushort Shirogane = 641;
    public const ushort Empyreum = 979;

    static ushort[] list = null;
    public static ushort[] List
    {
        get
        {
            if (list == null)
            {
                var s = new List<ushort>();
                typeof(ResidentalAreas).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Each(x => s.Add((ushort)x.GetValue(null)));
                list = s.ToArray();
            }
            return list;
        }
    }
}
