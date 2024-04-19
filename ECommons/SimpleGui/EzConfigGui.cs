using Dalamud.Configuration;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;

namespace ECommons.SimpleGui;
#nullable disable

public static class EzConfigGui
{
    public static WindowSystem WindowSystem { get; internal set; }
    internal static Action Draw = null;
    internal static Action OnClose = null;
    internal static Action OnOpen = null;
    internal static IPluginConfiguration Config;
    static ConfigWindow configWindow;
    public static Window Window { get { return configWindow; } }

    public static void Init(Action draw, IPluginConfiguration config = null)
    {
        Draw = draw;
        Init(config);
    }

    public static T Init<T>(T window, IPluginConfiguration config = null) where T:ConfigWindow
    {
        configWindow = window;
        Init(config);
        return window;
    }

    static void Init(IPluginConfiguration config)
    {
        if (WindowSystem != null)
        {
            throw new Exception("ConfigGui already initialized");
        }
        WindowSystem = new($"ECommons@{DalamudReflector.GetPluginName()}");
        Config = config;
        configWindow ??= new();
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
}
