using Dalamud.Logging;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.SplatoonAPI
{
    public static class Splatoon
    {
        internal static IDalamudPlugin Instance;
        internal static int Version;

        internal static void Init()
        {
            try
            {
                if (Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.IsLoaded").InvokeFunc())
                {
                    Connect();
                }
            }
            catch (Exception)
            {
            }
            Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Loaded").Subscribe(Connect);
            Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Unloaded").Subscribe(Reset);
        }

        internal static void Reset()
        {
            Instance = null;
            PluginLog.Information("Disconnected from Splatoon");
        }

        static void Connect()
        {
            try
            {
                DalamudReflector.TryGetDalamudPlugin("Splatoon", out var plugin);
                if ((bool)plugin.GetType().GetField("Init").GetValue(plugin))
                {
                    Instance = plugin;
                    Version++;
                    PluginLog.Information("Successfully connected to Splatoon.");
                }
                else
                {
                    throw new Exception("Splatoon is not initialized");
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("Can't find Splatoon plugin: " + e.Message);
                PluginLog.Error(e.StackTrace);
            }
        }
    }
}
