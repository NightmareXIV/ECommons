using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.ExcelServices;
#nullable disable

public static class ExcelWorldHelper
{
    [Obsolete("Please use Get")]
    public static World GetWorldByName(string name) => Get(name);

    private static Dictionary<string, World> NameCache = [];

    public static bool IsPublic(this World w)
    {
        if (w.IsPublic) return true;
        return w.RowId.EqualsAny<uint>(408, 409, 410, 411, 415);
    }

    public static World Get(string name, bool onlyPublic = false)
    {
        if (name == null) return null;
        if(NameCache.TryGetValue(name, out var world)) return world;
        if(Svc.Data.GetExcelSheet<World>().TryGetFirst(x => x.Name.ToString().EqualsIgnoreCase(name) && (!onlyPublic || x.Region.EqualsAny(Enum.GetValues<Region>().Select(z => (byte)z).ToArray())), out var result))
        {
            NameCache[name] = result;
						return result;
        }
        return null;
    }

    public static World Get(uint id, bool onlyPublic = false)
    {
        var result = Svc.Data.GetExcelSheet<World>().GetRow(id);
        if (result != null && (!onlyPublic || result.Region.EqualsAny(Enum.GetValues<Region>().Select(z => (byte)z).ToArray())))
        {
            return result;
        }
        return null;
    }

    [Obsolete("Please use TryGet")]
    public static bool TryGetWorldByName(string name, out World result) => TryGet(name, out result);

    public static bool TryGet(string name, out World result)
    {
        result = Get(name);
        return result != null;
    }

    public static bool TryGet(uint id, out World result)
    {
        result = Get(id);
        return result != null;
    }

    public static World[] GetPublicWorlds(Region? region = null)
    {
        return Svc.Data.GetExcelSheet<World>().Where(x => x.IsPublic() && (region == null || x.GetRegion() == region.Value)).ToArray();
    }

    public static World[] GetPublicWorlds(uint dataCenter)
    {
        return Svc.Data.GetExcelSheet<World>().Where(x => x.IsPublic() && x.DataCenter.Row == dataCenter).ToArray();
    }

    public static WorldDCGroupType[] GetDataCenters(Region? region = null)
    {
        return Svc.Data.GetExcelSheet<WorldDCGroupType>().Where(x => region == null || (Region)x.Region == region.Value).ToArray();
    }

    public static WorldDCGroupType[] GetDataCenters(System.Collections.Generic.IEnumerable<Region> regions)
    {
        return Svc.Data.GetExcelSheet<WorldDCGroupType>().Where(x => regions.Contains((Region)x.Region)).ToArray();
    }

    [Obsolete("Please use Get")]
    public static World GetWorldById(uint id) => Get(id);
    public static World Get(uint id)
    {
        return Svc.Data.GetExcelSheet<World>().GetRow(id);
    }

    [Obsolete("Please use GetName")]
    public static string GetWorldNameById(uint id) => GetName(id);
    public static string GetName(int id) => GetName((uint)id);
    public static string GetName(uint id)
    {
        return Get(id)?.Name.ToString();
    }

    [Obsolete("Please use Get")]
    public static World GetPublicWorldById(uint id) => Get(id, true);

    [Obsolete("Please use GetName")]
    public static string GetPublicWorldNameById(uint id) => Get(id, true).Name.ToString();

    [Obfuscation(Exclude = true)]
    public enum Region
    {
        JP = 1,
        NA = 2,
        EU = 3,
        OC = 4,
    }

    public static Region GetRegion(this World world)
    {
        var dc = world.DataCenter;
        var dcg = Svc.Data.GetExcelSheet<WorldDCGroupType>().GetRow(dc.Row);
        if (dcg == null) return 0;
        return (Region)dcg.Region;
    }
}
