using Dalamud;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Reflection
{
    public static class DalamudReflector
    {
        delegate ref int GetRefValue(int vkCode);
        static GetRefValue getRefValue;

        internal static void Init()
        {
            getRefValue = (GetRefValue)Delegate.CreateDelegate(typeof(GetRefValue), Svc.KeyState,
                        Svc.KeyState.GetType().GetMethod("GetRefValue",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int) }, null));
        }

        public static void SetKeyState(VirtualKey key, int state)
        {
            getRefValue((int)key) = state;
        }

        public static bool TryGetDalamudPlugin(string internalName, out IDalamudPlugin instance, bool suppressErrors = false)
        {
            try
            {
                var pluginManager = Svc.PluginInterface.GetType().Assembly.
                    GetType("Dalamud.Service`1", true).MakeGenericType(Svc.PluginInterface.GetType().Assembly.GetType("Dalamud.Plugin.Internal.PluginManager", true)).
                    GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
                var installedPlugins = (System.Collections.IList)pluginManager.GetType().GetProperty("InstalledPlugins").GetValue(pluginManager);

                foreach (var t in installedPlugins)
                {
                    if ((string)t.GetType().GetProperty("Name").GetValue(t) == internalName)
                    {
                        var type = t.GetType().Name == "LocalDevPlugin" ? t.GetType().BaseType : t.GetType();
                        var plugin = (IDalamudPlugin)type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);
                        instance = plugin;
                        return true;
                    }
                }
                instance = null;
                return false;
            }
            catch (Exception e)
            {
                if (!suppressErrors)
                {
                    PluginLog.Error($"Can't find {internalName} plugin: " + e.Message);
                    PluginLog.Error(e.StackTrace);
                }
                instance = null;
                return false;
            }
        }

        public static bool TryGetDalamudStartInfo(out DalamudStartInfo dalamudStartInfo)
        {
            try
            {
                var info = Svc.PluginInterface.GetType().Assembly.
                        GetType("Dalamud.Service`1", true).MakeGenericType(typeof(DalamudStartInfo)).
                        GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
                dalamudStartInfo = (DalamudStartInfo)info;
                return true;
            }
            catch (Exception e)
            {
                PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
                dalamudStartInfo = default;
                return false;
            }
        } 
    }
}
