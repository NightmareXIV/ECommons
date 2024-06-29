using ECommons.Logging;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.ImGuiMethods;
using ECommons.ObjectLifeTracker;
using ECommons.Reflection;
using ECommons.SimpleGui;
using ECommons.SplatoonAPI;
using ECommons.Events;
using ECommons.Configuration;
using ECommons.Hooks;
using ECommons.Loader;
using ECommons.Automation;
using ECommons.StringHelpers;
using ECommons.Commands;
using ECommons.Throttlers;
using ECommons.EzEventManager;
using ECommons.EzHookManager;
using ECommons.EzSharedDataManager;
using Serilog.Events;
using ECommons.EzIpcManager;
using System;
using System.Reflection;
using ECommons.Singletons;
using System.Linq;


#nullable disable

namespace ECommons;

public static class ECommonsMain
{
    public static IDalamudPlugin Instance = null;
    public static bool Disposed { get; private set; } = false;
    //test
    public static void Init(IDalamudPluginInterface pluginInterface, IDalamudPlugin instance, params Module[] modules)
    {
        Instance = instance;
        GenericHelpers.Safe(() => Svc.Init(pluginInterface));
#if DEBUG
var type = "debug build without forms";
#elif RELEASE
var type = "release build without forms";
#elif DEBUGFORMS
var type = "debug build with forms";
#elif RELEASEFORMS
var type = "release build with forms";
#else
var type = "unknown build";
#endif
        PluginLog.Information($"This is ECommons v{typeof(ECommonsMain).Assembly.GetName().Version} ({type}) and {Svc.PluginInterface.InternalName} v{instance.GetType().Assembly.GetName().Version}. Hello!");
        Svc.Log.MinimumLogLevel = LogEventLevel.Verbose;
        GenericHelpers.Safe(CmdManager.Init);
        if (modules.ContainsAny(Module.All, Module.ObjectFunctions))
        {
            PluginLog.Information("Object functions module has been requested");
            GenericHelpers.Safe(ObjectFunctions.Init);
        }
        if (modules.ContainsAny(Module.All, Module.DalamudReflector, Module.SplatoonAPI))
        {
            PluginLog.Information("Advanced Dalamud reflection module has been requested");
            GenericHelpers.Safe(() => DalamudReflector.Init());
        }
        if (modules.ContainsAny(Module.All, Module.ObjectLife))
        {
            PluginLog.Information("Object life module has been requested");
            GenericHelpers.Safe(ObjectLife.Init);
        }
        if(modules.ContainsAny(Module.All, Module.SplatoonAPI))
        {
            PluginLog.Information("Splatoon API module has been requested");
            GenericHelpers.Safe(Splatoon.Init);
        }
    }

    public static void CheckForObfuscation()
    {
        if(Assembly.GetCallingAssembly().GetTypes().FirstOrDefault(x => x.IsAssignableTo(typeof(IDalamudPlugin))).Name == Svc.PluginInterface.InternalName)
        {
            DuoLog.Error($"{Svc.PluginInterface.InternalName} name match error!");
        }
    }

    public static void Dispose()
    {
        Disposed = true;
				GenericHelpers.Safe(SingletonServiceManager.DisposeAll);
				GenericHelpers.Safe(PluginLoader.Dispose);
        GenericHelpers.Safe(CmdManager.Dispose);
        if (EzConfig.Config != null)
        {
            GenericHelpers.Safe(EzConfig.Save);
        }
        GenericHelpers.Safe(EzConfig.Dispose);
        GenericHelpers.Safe(ThreadLoadImageHandler.ClearAll);
        GenericHelpers.Safe(ObjectLife.Dispose);
        GenericHelpers.Safe(DalamudReflector.Dispose);
        if (EzConfigGui.WindowSystem != null)
        {
            Svc.PluginInterface.UiBuilder.OpenConfigUi -= EzConfigGui.Open;
            Svc.PluginInterface.UiBuilder.Draw -= EzConfigGui.Draw;
            if (EzConfigGui.Config != null)
            {
                Svc.PluginInterface.SavePluginConfig(EzConfigGui.Config);
                Notify.Info("Configuration saved");
            }
            EzConfigGui.WindowSystem.RemoveAllWindows();
            EzConfigGui.WindowSystem = null;
        }
        foreach (var x in EzCmd.RegisteredCommands)
        {
            Svc.Commands.RemoveHandler(x);
        }
        if (Splatoon.Instance != null)
        {
            GenericHelpers.Safe(Splatoon.Reset);
        }
        GenericHelpers.Safe(Splatoon.Shutdown);
        GenericHelpers.Safe(ProperOnLogin.Dispose);
        GenericHelpers.Safe(DirectorUpdate.Dispose);
        GenericHelpers.Safe(ActionEffect.Dispose);
        GenericHelpers.Safe(MapEffect.Dispose);
        GenericHelpers.Safe(SendAction.Dispose);
        GenericHelpers.Safe(Automation.LegacyTaskManager.TaskManager.DisposeAll);
        GenericHelpers.Safe(Automation.NeoTaskManager.TaskManager.DisposeAll);
        GenericHelpers.Safe(EqualStrings.Dispose);
        GenericHelpers.Safe(AutoCutsceneSkipper.Dispose);
        GenericHelpers.Safe(() => ThreadLoadImageHandler.httpClient?.Dispose());
        EzThrottler.Throttler = null;
        FrameThrottler.Throttler = null;
        GenericHelpers.Safe(Callback.Dispose);
        GenericHelpers.Safe(EzEvent.DisposeAll);
        GenericHelpers.Safe(EzHookCommon.DisposeAll);
        GenericHelpers.Safe(EzSharedData.Dispose);
        GenericHelpers.Safe(EzIPC.Dispose);
        //SingletonManager.Dispose();
        Chat.instance = null;
        Instance = null;
    }
}
