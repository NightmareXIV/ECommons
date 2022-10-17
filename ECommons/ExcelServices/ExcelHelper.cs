using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ExcelServices
{
    public static class ExcelHelper
    {
        public static World GetWorldByName(string name)
        {
            if(Svc.Data.GetExcelSheet<World>().TryGetFirst(x => x.Name.ToString().EqualsIgnoreCase(name), out var result))
            {
                return result;
            }
            return null;
        }

        public static bool TryGetWorldByName(string name, out World result)
        {
            result = GetWorldByName(name);
            return result != null;
        }

        public static string[] GetPublicWorlds(Region? region)
        {
            return Svc.Data.GetExcelSheet<World>().Where(x => (region == null && x.Region.EqualsAny(Enum.GetValues<Region>().Select(z => (byte)z).ToArray())) || (region.HasValue && x.Region == (byte)region.Value)).Select(x => x.Name.ToString()).ToArray();
        }
    }
}
