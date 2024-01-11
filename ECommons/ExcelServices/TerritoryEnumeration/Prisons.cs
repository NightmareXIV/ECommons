using System.Collections.Generic;
using System.Reflection;
#nullable disable

namespace ECommons.ExcelServices.TerritoryEnumeration;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public static class Prisons
{
    public const ushort Mordion_Gaol = 176;

    static ushort[] list = null;
    public static ushort[] List
    {
        get
        {
            if (list == null)
            {
                var s = new List<ushort>();
                typeof(Prisons).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Each(x => s.Add((ushort)x.GetValue(null)));
                list = s.ToArray();
            }
            return list;
        }
    }
}
