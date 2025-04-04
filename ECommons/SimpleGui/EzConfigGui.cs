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
    public static Window? Window { get; private set; }
    public static WindowType Type { get; private set; }
    internal static Action Draw = null;
    internal static Action OnClose = null;
    internal static Action OnOpen = null;
    internal static IPluginConfiguration Config;

    /// <summary>
    /// Initialize the EzConfig WindowSystem
    /// </summary>
    /// <param name="draw">Draw method. If initialized this way, a new instance of <see cref="ConfigWindow"/> will be created. Any window parameters will need to be set in separate method instead of the constructor.</param>
    /// <param name="config">Config to auto save on close</param>
    /// <param name="nameOverride">Override for the titlebar name. Default value is {InternalName v0.0.0.0}</param>
    /// <param name="windowType">Determines which UiBuilder event to subscribe to</param>
    public static void Init(Action draw, IPluginConfiguration config = null, string nameOverride = null, WindowType windowType = WindowType.Config)
    {
        Draw = draw;
        Init(config, nameOverride, windowType);
    }

    /// <summary>
    /// Initialize the EzConfig WindowSystem
    /// </summary>
    /// <param name="window">Window instance. If window is not of type <see cref="ConfigWindow"/> then config autosaving will not function.</param>
    /// <param name="config">Config to auto save on close</param>
    /// <param name="nameOverride">Override for the titlebar name. Default value is {InternalName v0.0.0.0}</param>
    /// <param name="windowType">Determines which UiBuilder event to subscribe to</param>
    public static T Init<T>(T window, IPluginConfiguration config = null, string nameOverride = null, WindowType windowType = WindowType.Config) where T : Window
    {
        Window = window;
        Init(config, nameOverride, windowType);
        return window;
    }

    private static void Init(IPluginConfiguration config, string nameOverride = null, WindowType windowType = WindowType.Config)
    {
        if(WindowSystem != null)
        {
            throw new Exception("ConfigGui already initialized");
        }
        WindowSystem = new($"ECommons@{DalamudReflector.GetPluginName()}");
        Config = config;
        Window ??= new ConfigWindow(nameOverride);
        WindowSystem.AddWindow(Window);
        Type = windowType;
        Svc.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        switch(windowType)
        {
            case WindowType.Config:
                Svc.PluginInterface.UiBuilder.OpenConfigUi += Open;
                break;
            case WindowType.Main:
                Svc.PluginInterface.UiBuilder.OpenMainUi += Open;
                break;
            case WindowType.Both:
                Svc.PluginInterface.UiBuilder.OpenConfigUi += Open;
                Svc.PluginInterface.UiBuilder.OpenMainUi += Open;
                break;
        }
    }

    public static void Open()
    {
        if(Window is not null)
            Window.IsOpen = true;
    }

    public static void Open(string cmd = null, string args = null)
    {
        Open();
    }

    public static void Toggle() => Window?.Toggle();

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
        if(GetWindow<T>() is { } window)
            WindowSystem.RemoveWindow(window);
    }

    public enum WindowType
    {
        Config,
        Main,
        Both
    }
}
