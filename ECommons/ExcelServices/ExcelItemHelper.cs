using AutoRetainer.Sheets;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace ECommons.ExcelServices;
#nullable disable

public static class ExcelItemHelper
{
    public static readonly Job[] Tanks = [Job.GLA, Job.PLD, Job.MRD, Job.WAR, Job.DRK, Job.GNB];
    public static readonly Job[] Healers = [Job.CNJ, Job.WHM, Job.SGE, Job.SCH, Job.AST];
    public static readonly Job[] StrengthDPS = [Job.PGL, Job.MNK, Job.LNC, Job.DRG, Job.SAM, Job.RPR];
    public static readonly Job[] DexterityDPS = [Job.BRD, Job.DNC, Job.MCH, Job.ROG, Job.NIN];
    public static readonly Job[] MagicalDPS = [Job.THM, Job.BLM, Job.ARC, Job.SMN, Job.RDM, Job.BLU];
    public static readonly Job[] Crafters = [Job.BRD, Job.DNC, Job.MCH, Job.ROG, Job.NIN];
    public static readonly Job[] Gatherers = [Job.BRD, Job.DNC, Job.MCH, Job.ROG, Job.NIN];

    public static readonly uint[] SuitableParamsForTanks = [1, 3, 27, 22, 19, 45, 44];
    public static readonly uint[] SuitableParamsForHealers = [];
    public static readonly uint[] SuitableParamsForStrengthDPS = [];

    public static HashSet<Job> GetSuitableJobsForItem(this ExtendedItem item)
    {
        var ret = new HashSet<Job>();
        var param = item.BaseParam[0];
        if (param.BaseParam.Row.EqualsAny<uint>(3, 19)) ret.Add(Tanks);
        if (param.BaseParam.Row.EqualsAny<uint>(5, 6)) ret.Add(Healers);
        if (param.BaseParam.Row.EqualsAny<uint>(1)) ret.Add(StrengthDPS);
        if (param.BaseParam.Row.EqualsAny<uint>(2)) ret.Add(DexterityDPS);
        if (param.BaseParam.Row.EqualsAny<uint>(4)) ret.Add(MagicalDPS);
        if (param.BaseParam.Row.EqualsAny<uint>(72, 73, 10)) ret.Add(Gatherers);
        if (param.BaseParam.Row.EqualsAny<uint>(70,71,11)) ret.Add(MagicalDPS);
        return ret;
    }

    public static string GetName(uint id, bool includeID = false)
    {
        var data = Svc.Data.GetExcelSheet<Item>().GetRow(id);
        if (data == null) return $"#{id}";
        return GetName(data);
    }

    static Dictionary<uint, string> ItemNameCache = [];
    public static string GetName(this Item item, bool includeID = false)
    {
        if (item == null) return "? Unknown ?";
        if (!ItemNameCache.TryGetValue(item.RowId, out string name))
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
