using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace ECommons.ExcelServices;

public static class ExcelItemHelper
{
    /// <summary>
    /// Gets <see cref="Item"/> by id or null if not found.
    /// </summary>
    /// <param name="rowId"></param>
    /// <returns></returns>
    public static Item? Get(int rowId) => Get((uint)rowId);

    /// <summary>
    /// Gets <see cref="Item"/> by id or null if not found.
    /// </summary>
    /// <param name="rowId"></param>
    /// <returns></returns>
    public static Item? Get(uint rowId) => Svc.Data.GetExcelSheet<Item>()!.GetRowOrDefault(rowId);

    /// <summary>
    /// Gets item name. If name or item is missing, prints item's ID. Item names are stripped off non-text payloads. Results are cached.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeID">Force include ID into text</param>
    /// <returns></returns>
    public static string GetName(uint id, bool includeID = false)
    {
        if(ItemNameCache.TryGetValue(id, out var ret)) return ret;
        var data = Svc.Data.GetExcelSheet<Item>()!.GetRowOrDefault(id);
        if(data == null) return $"#{id}";
        return GetName(data);
    }

    private static Dictionary<uint, string> ItemNameCache = [];
    /// <summary>
    /// Gets item name. If name is missing, prints item's ID. Item names are stripped off non-text payloads. Results are cached.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="includeID"></param>
    /// <returns></returns>
    public static string GetName(this Item? item, bool includeID = false)
    {
        if(item == null) return "? Unknown ?";
        if(!ItemNameCache.TryGetValue(item.Value.RowId, out var name))
        {
            name = item.Value.Name.ExtractText();
            ItemNameCache[item.Value.RowId] = name;
        }
        if(name == "")
        {
            return $"#{item.Value.RowId}";
        }
        return name;
    }
    public static string GetName(this Item item, bool includeID = false) => GetName((Item?)item, includeID);
}
