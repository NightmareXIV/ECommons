using ECommons.Logging;
using ECommons.ExcelServices.TerritoryEnumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ExcelServices
{
    public static class ExcelTerritoryHelper
    {
        static uint[] Sanctuaries = null;

        public static bool IsSanctuary(uint territoryType)
        {
            if(Sanctuaries == null)
            {
                var f = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
                var s = new List<uint>();
                typeof(MainCities).GetFields(f)
                    .Union(typeof(Inns).GetFields(f))
                    .Union(typeof(ResidentalAreas).GetFields(f))
                    .Union(typeof(Houses).GetFields(f))
                    .Each(x =>
                {
                    var v = (ushort)x.GetValue(null);
                    s.Add(v);
                    PluginLog.Verbose($"Sanctuary territory added: {x.Name} = {v}");
                });
                
                Sanctuaries = s.ToArray();
            }

            return Sanctuaries.Contains(territoryType);
        }
    }
}
