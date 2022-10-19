using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ExcelServices
{
    public unsafe static class ExcelActionHelper
    {
        public static float GetActionCooldown(uint id)
        {
            var detail = ActionManager.Instance()->GetRecastGroupDetail(Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(id).CooldownGroup - 1);
            return detail->IsActive == 1 ? detail->Total - detail->Elapsed : 0;
        }
    }
}
