using System.Collections.Generic;
using System.Reflection;
#nullable disable

namespace ECommons.ExcelServices.TerritoryEnumeration;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public static class VariantDungeons
{
    public const ushort The_Sildihn_Subterrane = 1069;

    static ushort[] list = null;
    public static ushort[] List
    {
        get
        {
            if (list == null)
            {
                var s = new List<ushort>();
                typeof(VariantDungeons).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Each(x => s.Add((ushort)x.GetValue(null)));
                list = s.ToArray();
            }
            return list;
        }
    }
}
