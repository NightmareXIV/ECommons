using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dalamud.Plugin.Services.IFramework;

namespace ECommons.EzEventManager
{
    internal static class EzEvent
    {
        internal static void DisposeAll()
        {
            EzFrameworkUpdate.Registered.Each(x => x.Dispose());
            EzFrameworkUpdate.Registered = null;
            EzLogout.Registered.Each(x => x.Dispose());
            EzLogout.Registered = null;
            EzTerritoryChanged.Registered.Each(x => x.Dispose());
            EzTerritoryChanged.Registered = null;
        }
    }
}
