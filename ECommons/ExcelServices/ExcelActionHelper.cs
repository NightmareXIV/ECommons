using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace ECommons.ExcelServices;
#nullable disable

public unsafe static class ExcelActionHelper
{
    public static float GetActionCooldown(uint id)
    {
        var detail = ActionManager.Instance()->GetRecastGroupDetail(Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(id).CooldownGroup - 1);
        var cdg2 = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(id).AdditionalCooldownGroup - 1;
        var ret = detail->IsActive == 1 ? detail->Total - detail->Elapsed : 0;
        if (cdg2 > 0)
        {
            var detail2 = ActionManager.Instance()->GetRecastGroupDetail(cdg2);
            var cd2 = detail2->IsActive == 1 ? detail2->Total - detail2->Elapsed : 0;
            return Math.Max(cd2, ret);
        }
        return ret;
    }

    public static string GetActionName(this Lumina.Excel.GeneratedSheets.Action data, bool forceIncludeID = false)
    {
        if(data == null)
        {
            return $"null";
        }
        else
        {
            var name = data.Name?.ExtractText();
            if (name.IsNullOrEmpty())
            {
                return $"#{data.RowId}";
            }
            else
            {
                return (forceIncludeID? $"#{data.RowId} ":$"") + name;
            }
        }
    }

    public static string GetActionName(uint id, bool forceIncludeID = false)
    {
        var d = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(id);
        if(d == null) return $"#{id}";
        return d.GetActionName(forceIncludeID);
    }
}
