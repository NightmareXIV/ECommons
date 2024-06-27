using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.ExcelServices;
#nullable disable

public static class ExcelJobHelper
{
    public static readonly Dictionary<Job, Job> Upgrades = new()
    {
        [Job.GLA] = Job.PLD,
        [Job.PGL] = Job.MNK,
        [Job.MRD] = Job.WAR,
        [Job.LNC] = Job.DRG,
        [Job.ARC] = Job.BRD,
        [Job.CNJ] = Job.WHM,
        [Job.THM] = Job.BLM,
        [Job.ACN] = Job.SMN,
        [Job.ROG] = Job.NIN,
    };

    public static Job GetUpgradedJob(this Job j)
    {
        if (Upgrades.TryGetValue(j, out Job job)) return job;
        return j;
    }

    public static Job GetDowngradedJob(this Job j)
    {
        var dj = Upgrades.FindKeysByValue(j);
        if(dj.TryGetFirst(out var ret))
        {
            return ret;
        }
        return j;
    }

    public static bool IsUpgradeable(this Job j) => Upgrades.ContainsKey(j);

    public static int GetIcon(this Job j)
    {
        return j == Job.ADV ? 62143 : (062100 + (int)j);
    }

    public static ClassJob GetJobByName(string name)
    {
        if (Svc.Data.GetExcelSheet<ClassJob>().TryGetFirst(x => x.Name.ToString().EqualsIgnoreCase(name), out var result))
        {
            return result;
        }
        return null;
    }

    public static bool TryGetJobByName(string name, out ClassJob result)
    {
        result = GetJobByName(name);
        return result != null;
    }

    public static ClassJob GetJobById(uint id)
    {
        return Svc.Data.GetExcelSheet<ClassJob>().GetRow(id);
    }

    public static string GetJobNameById(uint id)
    {
        return GetJobById(id)?.Name.ToString();
    }

    public static ClassJob[] GetCombatJobs()
    {
        return Svc.Data.GetExcelSheet<ClassJob>().Where(x => x.Role.EqualsAny<byte>(2, 3)).ToArray();
    }

    public static bool IsJobInCategory(this ClassJobCategory cat, Job job)
    {
        if (job == Job.ADV && cat.ADV) return true;
        if (job == Job.GLA && cat.GLA) return true;
        if (job == Job.PGL && cat.PGL) return true;
        if (job == Job.MRD && cat.MRD) return true;
        if (job == Job.LNC && cat.LNC) return true;
        if (job == Job.ARC && cat.ARC) return true;
        if (job == Job.CNJ && cat.CNJ) return true;
        if (job == Job.THM && cat.THM) return true;
        if (job == Job.CRP && cat.CRP) return true;
        if (job == Job.BSM && cat.BSM) return true;
        if (job == Job.ARM && cat.ARM) return true;
        if (job == Job.GSM && cat.GSM) return true;
        if (job == Job.LTW && cat.LTW) return true;
        if (job == Job.WVR && cat.WVR) return true;
        if (job == Job.ALC && cat.ALC) return true;
        if (job == Job.CUL && cat.CUL) return true;
        if (job == Job.MIN && cat.MIN) return true;
        if (job == Job.BTN && cat.BTN) return true;
        if (job == Job.FSH && cat.FSH) return true;
        if (job == Job.PLD && cat.PLD) return true;
        if (job == Job.MNK && cat.MNK) return true;
        if (job == Job.WAR && cat.WAR) return true;
        if (job == Job.DRG && cat.DRG) return true;
        if (job == Job.BRD && cat.BRD) return true;
        if (job == Job.WHM && cat.WHM) return true;
        if (job == Job.BLM && cat.BLM) return true;
        if (job == Job.ACN && cat.ACN) return true;
        if (job == Job.SMN && cat.SMN) return true;
        if (job == Job.SCH && cat.SCH) return true;
        if (job == Job.ROG && cat.ROG) return true;
        if (job == Job.NIN && cat.NIN) return true;
        if (job == Job.MCH && cat.MCH) return true;
        if (job == Job.DRK && cat.DRK) return true;
        if (job == Job.AST && cat.AST) return true;
        if (job == Job.SAM && cat.SAM) return true;
        if (job == Job.RDM && cat.RDM) return true;
        if (job == Job.BLU && cat.BLU) return true;
        if (job == Job.GNB && cat.GNB) return true;
        if (job == Job.DNC && cat.DNC) return true;
        if (job == Job.RPR && cat.RPR) return true;
        if (job == Job.SGE && cat.SGE) return true;
        return false;
    }
}
