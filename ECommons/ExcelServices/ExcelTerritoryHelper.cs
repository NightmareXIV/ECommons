﻿using ECommons.Logging;
using ECommons.ExcelServices.TerritoryEnumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
#nullable disable

namespace ECommons.ExcelServices;

public static class ExcelTerritoryHelper
{
    static uint[] Sanctuaries = null;

    /// <summary>
    /// Checks if territory belongs to main cities, inns, residential areas or houses. 
    /// </summary>
    /// <param name="territoryType"></param>
    /// <returns></returns>
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

    public static bool NameExists(uint TerritoryType)
    {
        var data = Svc.Data.GetExcelSheet<TerritoryType>().GetRow(TerritoryType);
        if (data != null) return NameExists(data);
        return false;
    }

    public static bool NameExists(this TerritoryType t)
    {
        var nonExists = t.Name.ExtractText().IsNullOrEmpty() && t.ContentFinderCondition?.Value.Name.ExtractText().IsNullOrEmpty() != false;
        return !nonExists;
    }

    /// <summary>
    /// Gets fancy name for a territory.
    /// </summary>
    /// <param name="TerritoryType">Zone ID</param>
    /// <param name="includeID">Whether to include an ID into name</param>
    /// <returns>Content finder condition if exists; otherwise - zone name if exists; otherwise - zone ID as a string</returns>
    public static string GetName(uint TerritoryType, bool includeID = false)
    {
        var data = Svc.Data.GetExcelSheet<TerritoryType>().GetRow(TerritoryType);
        string id = includeID?$"#{TerritoryType} | ":"";
        if (data == null) return $"#{TerritoryType}";
        var tname = data.PlaceName.Value?.Name?.ToString();
        var cfc = data.ContentFinderCondition.Value.Name?.ToString();
        if (cfc.IsNullOrEmpty())
        {
            if (tname.IsNullOrEmpty())
            {
                return $"#{TerritoryType}";
            }
            else
            {
                return $"{id}{tname}";
            }
        }
        else
        {
            return $"{id}{cfc}";
        }
    }

    public static TerritoryType Get(uint ID) => Svc.Data.GetExcelSheet<TerritoryType>().GetRow(ID);

    public static string GetBG(this TerritoryType t) => t?.Bg?.ExtractText();

    public static string GetBG(uint ID) => Get(ID)?.Bg?.ExtractText();
}
