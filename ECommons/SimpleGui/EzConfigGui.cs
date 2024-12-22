using Dalamud.Configuration;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Linq;

namespace ECommons.SimpleGui;
#nullable disable

public static class EzConfigGui
{
    public static WindowSystem WindowSystem { get; internal set; }
    internal static Action Draw = null;
    internal static Action OnClose = null;
    internal static Action OnOpen = null;
    internal static IPluginConfiguration Config;
    private static ConfigWindow configWindow;
    public static Window Window { get { return configWindow; } }

    public static void Init(Action draw, IPluginConfiguration config = null, string nameOverride = null)
    {
        Draw = draw;
        Init(config, nameOverride);
    }

    public static T Init<T>(T window, IPluginConfiguration config = null) where T : ConfigWindow
    {
        configWindow = window;
        Init(config);
        return window;
    }

    private static void Init(IPluginConfiguration config, string nameOverride = null)
    {
        if(WindowSystem != null)
        {
            throw new Exception("ConfigGui already initialized");
        }
        WindowSystem = new($"ECommons@{DalamudReflector.GetPluginName()}");
        Config = config;
        configWindow ??= new(nameOverride);
        WindowSystem.AddWindow(configWindow);
        Svc.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += Open;
    }

    public static void Open()
    {
        configWindow.IsOpen = true;
    }

    public static void Open(string cmd = null, string args = null)
    {
        Open();
    }

    public static void Toggle() => configWindow.Toggle();

    /// <summary>
    /// Returns a window from the EzGui WindowSystem.
    /// </summary>
    public static T? GetWindow<T>() where T : Window
        => !typeof(T).IsSubclassOf(typeof(Window)) ? null : WindowSystem.Windows.FirstOrDefault(w => w.GetType() == typeof(T)) as T;

    /// <summary>
    /// Removes a window from the EzGui WindowSystem. Windows are auto-disposed upon plugin unload. This is only needed if you need to manually remove a window prior to plugin unload.
    /// </summary>
    public static void RemoveWindow<T>() where T : Window
    {
        if (!typeof(T).IsSubclassOf(typeof(Window))) return;
        var window = WindowSystem.Windows.FirstOrDefault(w => w.GetType() == typeof(T));
        if (window != null)
            WindowSystem.RemoveWindow(window);
    }
}
