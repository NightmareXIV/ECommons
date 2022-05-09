using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons
{
    public static class ECommons
    {
        public static void Init(DalamudPluginInterface pluginInterface)
        {
            Svc.Init(pluginInterface);
            ObjectFunctions.Init();
        }
    }
}
