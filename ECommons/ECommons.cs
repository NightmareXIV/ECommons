using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.ObjectLifeTracker;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons
{
    public static class ECommons
    {
        public static void Init(DalamudPluginInterface pluginInterface, bool initModules = true)
        {
            GenericHelpers.Safe(() => Svc.Init(pluginInterface));
            if (initModules)
            {
                GenericHelpers.Safe(ObjectFunctions.Init);
                GenericHelpers.Safe(DalamudReflector.Init); 
            }
        }

        public static void Dispose()
        {
            foreach(var x in ImGuiMethods.ThreadLoadImageHandler.CachedTextures)
            {
                GenericHelpers.Safe(x.Value.texture.Dispose);
            }
            GenericHelpers.Safe(ImGuiMethods.ThreadLoadImageHandler.CachedTextures.Clear);
            GenericHelpers.Safe(ObjectLife.Dispose);
        }
    }
}
