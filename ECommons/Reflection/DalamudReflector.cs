using Dalamud.Common;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.EzSharedDataManager;
using ECommons.Logging;
using ECommons.Schedulers;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using CapturedPluginState = (string InternalName, System.Version Version, bool IsLoaded);
using Task = System.Threading.Tasks.Task;

#nullable disable

namespace ECommons.Reflection;

public static class DalamudReflector
{
    private delegate ref int GetRefValue(int vkCode);

    private static GetRefValue getRefValue;
    private static Dictionary<string, CachedPluginEntry> pluginCache;
    private static List<Action> onPluginsChangedActions;
    private static bool IsMonitoring = false;

    internal static void Init()
    {
        onPluginsChangedActions = [];
        pluginCache = [];
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
        if(pluginCache != null)
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
        if(!IsMonitoring)
        {
            IsMonitoring = true;
            PluginLog.Information($"[ECommons] [DalamudReflector] RegisterOnInstalledPluginsChangedEvents was requested for the first time. Starting to monitor plugins for changes...");
            Svc.Framework.Update += MonitorPlugins;
        }
        foreach(var x in actions)
        {
            onPluginsChangedActions.Add(x);
        }
    }

    public static bool GetDtrEntryState(string name)
    {
        try
        {
            var config = DalamudReflector.GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
            var dtrList = config.GetFoP<List<string>>("DtrIgnore");
            var enabled = !dtrList.Contains(name);
            return enabled;
        }
        catch(Exception e)
        {
            e.Log();
        }
        return false;
    }

    public static void SetDtrEntryState(string name, bool enable)
    {
        try
        {
            var config = DalamudReflector.GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
            var dtrList = config.GetFoP<List<string>>("DtrIgnore");
            var enabled = !dtrList.Contains(name);
            if(enable && !enabled)
            {
                dtrList.Remove(name);
                config.Call("QueueSave", []);
            }
            else if(!enable && enabled)
            {
                dtrList.Add(name);
                config.Call("QueueSave", []);
            }
        }
        catch(Exception e)
        {
            e.Log();
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

    /// <summary>
    /// Loads a remote Plugin Master.
    /// </summary>
    /// <param name="masterURL">The URL to the remote Master.</param>
    /// <returns>
    /// A List of <see cref="Dalamud.Plugin.Internal.Types.Manifest.RemotePluginManifest"/>s for each plugin in the Master.<br/>
    /// List will be null if the operation fails.
    /// </returns>
    public static async Task<List<object>?> GetPluginMaster(string masterURL)
    {
        List<object>? plugins = null;

        try
        {
            var happyHttpClient = GetService("Dalamud.Networking.Http.HappyHttpClient");
            var pluginRepository = Activator.CreateInstance(
                Svc.PluginInterface.GetType().Assembly
                    .GetType("Dalamud.Plugin.Internal.Types.PluginRepository")!,
                happyHttpClient, masterURL, true);
            await pluginRepository.Call<Task>("ReloadPluginMasterAsync", []);

            var pluginMaster = pluginRepository!.GetType()
                .GetProperty("PluginMaster")!
                .GetValue(pluginRepository) as System.Collections.IEnumerable;

            plugins = pluginMaster?.Cast<object>().ToList();
        }
        catch(Exception e)
        {
            PluginLog.Error("[ECommons] [DalamudReflector] Failed to get plugin master:\n" + e.Message + "\n" + e.StackTrace);
            return null;
        }

        return plugins;
    }

    public static object GetService(string serviceFullName)
    {
        return Svc.PluginInterface.GetType().Assembly.
                GetType("Dalamud.Service`1", true).MakeGenericType(Svc.PluginInterface.GetType().Assembly.GetType(serviceFullName, true)).
                GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
    }

    private static CapturedPluginState[] PrevInstalledPluginState = [];

    private static void MonitorPlugins(object _)
    {
        if(!Svc.PluginInterface.InstalledPlugins.ExposedPluginsEqual(PrevInstalledPluginState))
        {
            PrevInstalledPluginState = Svc.PluginInterface.InstalledPlugins.Select(x => new CapturedPluginState(x.InternalName, x.Version, x.IsLoaded)).ToArray();
            OnInstalledPluginsChanged();
        }
    }

    private static bool ExposedPluginsEqual(this IEnumerable<IExposedPlugin> plugins, IEnumerable<CapturedPluginState> other)
    {
        if(plugins.Count() != other.Count()) return false;
        var enumeratorOriginal = plugins.GetEnumerator();
        var enumeratorOther = other.GetEnumerator();
        while(true)
        {
            var move1 = enumeratorOriginal.MoveNext();
            var move2 = enumeratorOther.MoveNext();
            if(move1 != move2) return false;
            if(move1 == false) return true;
            if(enumeratorOriginal.Current.IsLoaded != enumeratorOther.Current.IsLoaded) return false;
            if(enumeratorOriginal.Current.Version != enumeratorOther.Current.Version) return false;
            if(enumeratorOriginal.Current.InternalName != enumeratorOther.Current.InternalName) return false;
        }
    }

    public static bool TryGetLocalPlugin(out object localPlugin, out AssemblyLoadContext context, out Type type) =>
        TryGetLocalPlugin(ECommonsMain.Instance, out localPlugin, out context, out type);

    public static bool TryGetLocalPlugin(IDalamudPlugin instance, out object localPlugin, out AssemblyLoadContext context, out Type type)
    {
        try
        {
            if(ECommonsMain.Instance == null)
            {
                throw new Exception("PluginInterface is null. Did you initalise ECommons?");
            }
            var pluginManager = GetPluginManager();
            var installedPlugins = (System.Collections.IList)pluginManager.GetType().GetProperty("InstalledPlugins").GetValue(pluginManager);
            foreach(var t in installedPlugins)
            {
                type = t.GetType().Name == "LocalDevPlugin" ? t.GetType().BaseType : t.GetType();
                var plugin = (IDalamudPlugin)type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);
                if(plugin == ECommonsMain.Instance)
                {
                    localPlugin = t;
                    context = type.GetField("loader", ReflectionHelper.AllFlags).GetValue(t)?.GetFoP<AssemblyLoadContext>("context");
                    return true;
                }
            }
            type = null;
            context = null;
            localPlugin = null;
            return false;
        }
        catch(Exception e)
        {
            e.Log();
        }
        type = null;
        context = null;
        localPlugin = null;
        return false;
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
        if(!ignoreCache)
        {
            if(!IsMonitoring)
            {
                IsMonitoring = true;
                PluginLog.Information($"[ECommons] [DalamudReflector] Plugin cache was requested for the first time. Starting to monitor plugins for changes...");
                Svc.Framework.Update += MonitorPlugins;
            }
        }
        if(pluginCache == null)
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

            foreach(var t in installedPlugins)
            {
                if((string)t.GetType().GetProperty("InternalName").GetValue(t) == internalName)
                {
                    var type = t.GetType().Name == "LocalDevPlugin" ? t.GetType().BaseType : t.GetType();
                    var plugin = (IDalamudPlugin)type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);
                    if(plugin == null)
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
        catch(Exception e)
        {
            if(!suppressErrors)
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
            if(pluginInterface == null) pluginInterface = Svc.PluginInterface;
            var info = pluginInterface.GetType().Assembly.
                    GetType("Dalamud.Service`1", true).MakeGenericType(pluginInterface.GetType().Assembly.GetType("Dalamud.Dalamud", true)).
                    GetMethod("Get").Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
            dalamudStartInfo = info.GetFoP<DalamudStartInfo>("StartInfo");
            return true;
        }
        catch(Exception e)
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
        if(TryGetDalamudStartInfo(out var startinfo, Svc.PluginInterface))
        {
            if(File.Exists(startinfo.ConfigurationPath))
            {
                var file = File.ReadAllText(startinfo.ConfigurationPath);
                var ob = JsonConvert.DeserializeObject<dynamic>(file);
                string type = ob.DalamudBetaKind;
                if(type is not null && !string.IsNullOrEmpty(type) && type != "release")
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

    /// <summary>
    /// Checks the Dalamud Configuration for the presence of a given repository URL.
    /// </summary>
    public static bool HasRepo(string repoURL)
    {
        var conf = GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
        var repolist = (System.Collections.IEnumerable)conf.GetFoP("ThirdRepoList");
        if(repolist != null)
            foreach(var r in repolist)
                if((string)r.GetFoP("Url") == repoURL)
                    return true;
        return false;
    }

    /// <summary>
    /// Attempts to add a new repository entry into the Dalamud Configuration. If the repo already exists, nothing is overridden.
    /// </summary>
    /// <param name="repoURL">The json URL of the repository.</param>
    /// <param name="enabled">Set the enabled state, whether plugins from the repo will load in the plugin installer.</param>
    public static void AddRepo(string repoURL, bool enabled)
    {
        var conf = GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
        var repolist = (System.Collections.IEnumerable)conf.GetFoP("ThirdRepoList");
        if(repolist != null)
            foreach(var r in repolist)
                if((string)r.GetFoP("Url") == repoURL)
                    return;
        var instance = Activator.CreateInstance(Svc.PluginInterface.GetType().Assembly.GetType("Dalamud.Configuration.ThirdPartyRepoSettings")!);
        instance.SetFoP("Url", repoURL);
        instance.SetFoP("IsEnabled", enabled);
        conf.GetFoP<System.Collections.IList>("ThirdRepoList").Add(instance!);
    }

    /// <summary>
    /// Installs a plugin from a remote Plugin Master.
    /// </summary>
    /// <param name="masterURL">the remote Master for <see cref="GetPluginMaster"/>.</param>
    /// <param name="pluginInternalName">the internal name of the plugin you want to install.</param>
    /// <returns>whether the install succeeded.</returns>
    /// <remarks>
    /// Will provide Error logs for any type of failure.<br/>
    /// Will add the remote Master as a Repo if it isn't already one.<br/>
    /// Installs via <see cref="Dalamud.Plugin.Internal.PluginManager.InstallPluginAsync"/>.
    /// </remarks>
    public static async Task<bool> AddPlugin(string masterURL, string
            pluginInternalName)
    {
        // Get the remote manifest
        var plugins = await GetPluginMaster(masterURL);
        if(plugins == null || plugins.Count == 0)
            return false; // Will already have a log
        var pluginManifest = plugins.FirstOrDefault(x => (string)x.GetFoP("InternalName") == pluginInternalName);
        if(pluginManifest == null)
        {
            PluginLog.Error("[ECommons] [DalamudReflector] Failed to add plugin:\n" +
                            "Failed to find plugin in master:\n" +
                            $"Master URL: {masterURL}\n" +
                            $"Plugin Internal Name: {pluginInternalName}");
            return false;
        }

        object pm = null;
        var error = "";
        try { pm = GetPluginManager(); } catch(Exception e) { error = e.Message; }
        if(pm == null || error != "")
        {
            PluginLog.Error("[ECommons] [DalamudReflector] Failed to add plugin:\n" +
                            "Failed to get plugin manager:\n" +
                            error);
            return false;
        }

        // Install the plugin
        try
        {
            if(!HasRepo(masterURL))
                AddRepo(masterURL, true);
            ReloadPluginMasters(); // necessary to avoid it be listed as orphaned

            var installCall = pm.Call<Task>("InstallPluginAsync", [pluginManifest, false, PluginLoadReason.Installer, null]);
            await installCall;

            var localPlugin = installCall.GetFoP("Result");
            // A good check, but mostly here so that it will Throw if not installed
            if((bool)localPlugin.GetFoP("IsLoaded"))
            {
                return true;
            }
        }
        catch(Exception e)
        {
            PluginLog.Error("[ECommons] [DalamudReflector] Failed to add plugin:\n" +
                            "Failed to finish installing plugin:\n" +
                            e.Message + "\n" + e.StackTrace);
            return false;
        }

        PluginLog.Error("[ECommons] [DalamudReflector] Failed to add plugin:\n" +
                        "Unkown failure:\n" +
                        JsonConvert.SerializeObject(new { masterURL }));
        return false;
    }

    /// <summary>
    /// Unloads then Deletes the current plugin on the next Tick.<br/>
    /// The plugin that called this. Your plugin.
    /// </summary>
    /// <remarks>
    /// Uses the internal
    /// <see cref="Dalamud.Plugin.Internal.Types.LocalPlugin.UnloadAsync"/><br/>
    /// and <see cref="Dalamud.Plugin.Internal.PluginManager.RemovePlugin"/>
    /// </remarks>
    public static void RemoveCurrentPlugin()
    {
        object pm = null;
        var error = "";
        try { pm = GetPluginManager(); } catch(Exception e) { error = e.Message; }
        if(pm == null || error != "")
        {
            PluginLog.Error("[ECommons] [DalamudReflector] Failed to remove plugin:\n" +
                            "Failed to get plugin manager:\n" +
                            error);
            return;
        }

        // If you wanted to be able to delete other plugins, you would replace this
        // with functionality that creates a LocalPlugin instance based on the
        // plugin's InternalName and searching InstalledPlugins
        TryGetLocalPlugin(out var localPlugin, out _, out _);

        // Default unload mode
        var disposalMode = Enum.Parse(Svc.PluginInterface.GetType().Assembly.GetType
            ("Dalamud.Plugin.Internal.Types.PluginLoaderDisposalMode")!, "WaitBeforeDispose");

        // Save and run the Action that unloads+removes the plugin outside the plugin code
        Svc.Framework.RunOnTick((Action)UnloadAndRemove);
        return;

        // Actual series of events to unload and remove the plugin
        async void UnloadAndRemove()
        {
            var unloading = localPlugin.Call<Task>("UnloadAsync", [disposalMode]);
            await unloading;
            pm.Call("RemovePlugin", [localPlugin]);
        }
    }

    /// <summary>
    /// Reloads the Dalamud Plugin Manager, effectively the same as closing and reopening the Plugin Installer window.
    /// </summary>
    public static void ReloadPluginMasters()
    {
        var mgr = GetService("Dalamud.Plugin.Internal.PluginManager");
        var pluginReload = mgr?.GetType().GetMethod("SetPluginReposFromConfigAsync", BindingFlags.Instance | BindingFlags.Public);
        pluginReload?.Invoke(mgr, [true]);
    }

    /// <summary>
    /// Saves the Dalamud Configuration.
    /// </summary>
    public static void SaveDalamudConfig()
    {
        var conf = GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
        var configSave = conf?.GetType().GetMethod("QueueSave", BindingFlags.Instance | BindingFlags.Public);
        configSave?.Invoke(conf, null);
    }

    /// <summary>
    /// Deletes specified shared data
    /// </summary>
    /// <param name="name"></param>
    public static void DeleteSharedData(string name)
    {
        DalamudReflector.GetService("Dalamud.Plugin.Ipc.Internal.DataShare").GetFoP<System.Collections.IDictionary>("caches").Remove(name);
        EzSharedData.Cache.Remove(name);
    }
}
