using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using FFXIVClientStructs.FFXIV.Client.Game;
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

    public static int GetStat(this Item item, BaseParamEnum param, bool isHq = false)
    {
        var ret = 0;
        for(int i = 0; i < item.BaseParam.Count; i++)
        {
            if(item.BaseParam[i].RowId == (int)param)
            {
                if(isHq)
                {
                    if(item.BaseParamValueSpecial[i] > 0)
                    {
                        ret += item.BaseParamValueSpecial[i];
                        continue;
                    }
                }
                ret += item.BaseParamValue[i];
            }
        }
        return ret;
    }

    public static int GetStat(this InventoryItem item, BaseParamEnum param)
    {
        var ret = 0;
        if(ExcelItemHelper.Get(item.GetItemId()).TryGetValue(out var data))
        {
            ret += GetStat(data, param, item.Flags.HasFlag(InventoryItem.ItemFlags.HighQuality));
        }
        for(byte i = 0; i < item.GetMateriaCount(); i++)
        {
            var m = item.GetMateriaId(i);
            var grade = item.GetMateriaGrade(i);
            if(Svc.Data.GetExcelSheet<Materia>().TryGetRow(m, out var mData))
            {
                if(mData.BaseParam.RowId == (int)param)
                {
                    ret += mData.Value[grade];
                }
            }
        }
        return ret;
    }
}
