using ECommons.DalamudServices;
using Lumina.Data;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;

namespace ECommons;

public static class TerritoryName
{
    private static Dictionary<uint, string> Cache = [];

    public static string GetTerritoryName(uint id, Language? lang = null)
    {
        if(Cache.TryGetValue(id, out var val))
        {
            return val;
        }
        var data = Svc.Data.Excel.GetSheet<TerritoryType>(lang)!.GetRowOrDefault(id);
        if(data != null)
        {
            var zoneName = data.Value.PlaceName.ValueNullable?.Name.ToString() ?? "";
            if(zoneName != string.Empty)
            {
                var cfc = data.Value.ContentFinderCondition.ValueNullable;
                if(cfc != null)
                {
                    var cfcStr = cfc.Value.Name.ToString();
                    if(cfcStr != String.Empty)
                    {
                        Cache[id] = $"{id} | {zoneName} ({cfcStr})";
                        return Cache[id];
                    }
                }
                Cache[id] = $"{id} | {zoneName}";
                return Cache[id];
            }
        }
        Cache[id] = $"{id}";
        return Cache[id];
    }
}
