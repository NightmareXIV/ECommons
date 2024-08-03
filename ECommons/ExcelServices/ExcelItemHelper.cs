using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace ECommons.ExcelServices;
#nullable disable

public static class ExcelItemHelper
{
    public static Item Get(int rowId) => Get((uint)rowId);

    public static Item Get(uint rowId) => Svc.Data.GetExcelSheet<Item>().GetRow(rowId);

    public static string GetName(uint id, bool includeID = false)
    {
        var data = Svc.Data.GetExcelSheet<Item>().GetRow(id);
        if(data == null) return $"#{id}";
        return GetName(data);
    }

    private static Dictionary<uint, string> ItemNameCache = [];
    public static string GetName(this Item item, bool includeID = false)
    {
        if(item == null) return "? Unknown ?";
        if(!ItemNameCache.TryGetValue(item.RowId, out var name))
        {
            name = item.Name.ExtractText();
            ItemNameCache[item.RowId] = name;
        }
        if(name == "")
        {
            return $"#{item.RowId}";
        }
        return name;
    }
}
