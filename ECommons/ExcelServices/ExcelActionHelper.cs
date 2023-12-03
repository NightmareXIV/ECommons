using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace ECommons.ExcelServices;

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
}
