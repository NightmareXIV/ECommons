using Dalamud.Game.ClientState.Keys;
using ECommons.Logging;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.Schedulers;
using System;
using System.Collections.Generic;
using System.Reflection;
using Dalamud.Common;
using System.Linq;
using System.Runtime.Loader;
using Newtonsoft.Json;
using System.IO;
#nullable disable

namespace ECommons.Reflection;

public static class DalamudReflector
{
    delegate ref int GetRefValue(int vkCode);
    static GetRefValue getRefValue;
    static Dictionary<string, CachedPluginEntry> pluginCache;
    static List<Action> onPluginsChangedActions;
    static bool IsMonitoring = false;

    internal static void Init()
    {
        onPluginsChangedActions = new();
        pluginCache = new();
        GenericHelpers.Safe(delegate
        {
            getRefValue = (GetRefValue)Delegate.CreateDelegate(typeof(GetRefValue), Svc.KeyState,
                        Svc.KeyState.GetType().GetMethod("GetRefValue",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int) }, null));
        });
    }

    internal static void Dispose()
    {
        if (pluginCache != null)
        {
            pluginCache = null;
            onPluginsChangedActions = null;
        }
        Svc.Framework.Update -= MonitorPlugins;
    }

    /// <summary>
    /// Registers actions that will be triggered upon any installed plugin state change. Plugin monitoring will begin upon registering any actions.
    /// </summary>
    /// <param name="actions"></param>
    public static void RegisterOnInstalledPluginsChangedEvents(params Action[] actions)
    {
        if (!IsMonitoring)
        {
            IsMonitoring = true;
            PluginLog.Information($"[ECommons] [DalamudReflector] RegisterOnInstalledPluginsChangedEvents was requested for the first time. Starting to monitor plugins for changes...");
            Svc.Framework.Update += MonitorPlugins;
        }
        foreach (var x in actions)
        {
            onPluginsChangedActions.Add(x);
        }
    }


    public static void SetKeyState(VirtualKey key, int state)
    {
        getRefValue((int)key) = state;
    }

    public static object GetPluginManager()
    {
        return Svc.PluginInterface.GetType().Assembly.
                GetType("Dalamud.Service`1", true).MakeGenericType(Svc.PluginInterface.GetType().Assembly.GetType("Dalamud.Plugin.Internal.PluginManager", true)).
                GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
    }

    public static object GetService(string serviceFullName)
    {
        return Svc.PluginInterface.GetType().Assembly.
                GetType("Dalamud.Service`1", true).MakeGenericType(Svc.PluginInterface.GetType().Assembly.GetType(serviceFullName, true)).
                GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
    }

    static IExposedPlugin[] PrevInstalledPluginState = [];
    static void MonitorPlugins(object _)
    {
        if(!Svc.PluginInterface.InstalledPlugins.SequenceEqual(PrevInstalledPluginState))
        {
            PrevInstalledPluginState = Svc.PluginInterface.InstalledPlugins.ToArray();
            OnInstalledPluginsChanged();
        }
    }

    public static bool TryGetLocalPlugin(out object localPlugin, out Type type) => TryGetLocalPlugin(ECommonsMain.Instance, out localPlugin, out type);

    public static bool TryGetLocalPlugin(IDalamudPlugin instance, out object localPlugin, out Type type)
    {
        try
        {
            if (ECommonsMain.Instance == null)
            {
                throw new Exception("PluginInterface is null. Did you initalise ECommons?");
            }
            var pluginManager = GetPluginManager();
            var installedPlugins = (System.Collections.IList)pluginManager.GetType().GetProperty("InstalledPlugins").GetValue(pluginManager);

            foreach (var t in installedPlugins)
            {
                if (t != null)
                {
                    type = t.GetType().Name == "LocalDevPlugin" ? t.GetType().BaseType : t.GetType();
                    if (object.ReferenceEquals(type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t), instance))
                    {
                        localPlugin = t;
                        return true;
                    }
                }
            }
            localPlugin = type = null;
            return false;
        }
        catch(Exception e)
        {
            e.Log();
            localPlugin = type = null;
            return false;
        }
    }

    public static bool TryGetDalamudPlugin(string internalName, out IDalamudPlugin instance, bool suppressErrors = false, bool ignoreCache = false) => TryGetDalamudPlugin(internalName, out instance, out _, suppressErrors, ignoreCache);

    /// <summary>
    /// Attempts to retrieve an instance of loaded plugin and it's load context. 
    /// </summary>
    /// <param name="internalName">Target plugin's internal name</param>
    /// <param name="instance">Plugin instance</param>
    /// <param name="context">Plugin's load context. May be null.</param>
    /// <param name="suppressErrors">Whether to stay silent on failures</param>
    /// <param name="ignoreCache">Whether to disable caching of the plugin and it's context to speed up further searches</param>
    /// <returns>Whether operation succeeded</returns>
    /// <exception cref="Exception"></exception>
    public static bool TryGetDalamudPlugin(string internalName, out IDalamudPlugin instance, out AssemblyLoadContext context, bool suppressErrors = false, bool ignoreCache = false)
    {
        if (!ignoreCache)
        {
            if (!IsMonitoring)
            {
                IsMonitoring = true;
                PluginLog.Information($"[ECommons] [DalamudReflector] Plugin cache was requested for the first time. Starting to monitor plugins for changes...");
                Svc.Framework.Update += MonitorPlugins;
            }
        }
        if (pluginCache == null)
        {
            throw new Exception("PluginCache is null. Have you initialised the DalamudReflector module on ECommons initialisation?");
        }

        if(!ignoreCache && pluginCache.TryGetValue(internalName, out var entry) && entry.Plugin != null)
        {
            instance = entry.Plugin;
            context = entry.Context;
            return true;
        }
        try
        {
            var pluginManager = GetPluginManager();
            var installedPlugins = (System.Collections.IList)pluginManager.GetType().GetProperty("InstalledPlugins").GetValue(pluginManager);

            foreach (var t in installedPlugins)
            {
                if ((string)t.GetType().GetProperty("InternalName").GetValue(t) == internalName)
                {
                    var type = t.GetType().Name == "LocalDevPlugin" ? t.GetType().BaseType : t.GetType();
                    var plugin = (IDalamudPlugin)type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);
                    if (plugin == null)
                    {
                        InternalLog.Warning($"Found requested plugin {internalName} but it was null");
                    }
                    else
                    {
                        instance = plugin;
                        context = t.GetFoP("loader")?.GetFoP<AssemblyLoadContext>("context");
                        pluginCache[internalName] = new(plugin, context);
                        return true;
                    }
                }
            }
            instance = null;
            context = null;
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
            context = null;
            return false;
        }
    }
    
    public static bool TryGetDalamudStartInfo(out DalamudStartInfo dalamudStartInfo, IDalamudPluginInterface pluginInterface = null)
    {
        try
        {
            if (pluginInterface == null) pluginInterface = Svc.PluginInterface;
            var info = pluginInterface.GetType().Assembly.
                    GetType("Dalamud.Service`1", true).MakeGenericType(pluginInterface.GetType().Assembly.GetType("Dalamud.Dalamud", true)).
                    GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
            dalamudStartInfo = info.GetFoP<DalamudStartInfo>("StartInfo");
            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            dalamudStartInfo = default;
            return false;
        }
    }

    public static string GetPluginName()
    {
        return Svc.PluginInterface?.InternalName ?? "Not initialized";
    }

    internal static void OnInstalledPluginsChanged()
    {
        PluginLog.Verbose("Installed plugins changed event fired");
        _ = new TickScheduler(delegate
        {
            pluginCache.Clear();
            foreach(var x in onPluginsChangedActions)
            {
                x();
            }
        });
    }

    public static bool IsOnStaging()
    {
        if (TryGetDalamudStartInfo(out var startinfo, Svc.PluginInterface))
        {
            if (File.Exists(startinfo.ConfigurationPath))
            {
                var file = File.ReadAllText(startinfo.ConfigurationPath);
                var ob = JsonConvert.DeserializeObject<dynamic>(file);
                string type = ob.DalamudBetaKind;
                if (type is not null && !string.IsNullOrEmpty(type) && type != "release")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
