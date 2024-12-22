using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.Logging;
using System;
#nullable disable

namespace ECommons.Hooks;

public static class DirectorUpdate
{
    private static readonly string Sig = "48 89 5C 24 ?? 57 48 83 EC 30 41 8B D9 41 8B F8 E8 ?? ?? ?? ?? 48 85 C0";

    public delegate long ProcessDirectorUpdate(long a1, long a2, DirectorUpdateCategory a3, uint a4, uint a5, int a6, int a7);
    internal static Hook<ProcessDirectorUpdate> ProcessDirectorUpdateHook = null;
    private static Action<long, long, DirectorUpdateCategory, uint, uint, int, int> FullParamsCallback = null;
    private static Action<DirectorUpdateCategory> CategoryOnlyCallback = null;
    private static ProcessDirectorUpdate OriginalDelegate;
    public static ProcessDirectorUpdate Delegate
    {
        get
        {
            if(ProcessDirectorUpdateHook != null && !ProcessDirectorUpdateHook.IsDisposed)
            {
                return ProcessDirectorUpdateHook.Original;
            }
            else
            {
                OriginalDelegate ??= EzDelegate.Get<ProcessDirectorUpdate>(Sig);
                return OriginalDelegate;
            }
        }
    }

    internal static long ProcessDirectorUpdateDetour_Full(long a1, long a2, DirectorUpdateCategory a3, uint a4, uint a5, int a6, int a7)
    {
        try
        {
            FullParamsCallback(a1, a2, a3, a4, a5, a6, a7);
        }
        catch(Exception e)
        {
            e.Log();
        }
        return ProcessDirectorUpdateHook.Original(a1, a2, a3, a4, a5, a6, a7);
    }

    internal static long ProcessDirectorUpdateDetour_Category(long a1, long a2, DirectorUpdateCategory a3, uint a4, uint a5, int a6, int a7)
    {
        try
        {
            CategoryOnlyCallback(a3);
        }
        catch(Exception e)
        {
            e.Log();
        }
        return ProcessDirectorUpdateHook.Original(a1, a2, a3, a4, a5, a6, a7);
    }

    public static void Init(Action<long, long, DirectorUpdateCategory, uint, uint, int, int> fullParamsCallback)
    {
        if(ProcessDirectorUpdateHook != null)
        {
            throw new Exception("Director Update Hook is already initialized!");
        }
        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            FullParamsCallback = fullParamsCallback;
            ProcessDirectorUpdateHook = Svc.Hook.HookFromAddress<ProcessDirectorUpdate>(ptr, ProcessDirectorUpdateDetour_Full);
            Enable();
            PluginLog.Information($"Requested Director Update hook and successfully initialized with FULL data");
        }
        else
        {
            PluginLog.Error($"Could not find DirectorUpdate signature");
        }
    }

    public static void Init(Action<DirectorUpdateCategory> categoryOnlyCallback)
    {
        if(ProcessDirectorUpdateHook != null)
        {
            throw new Exception("Director Update Hook is already initialized!");
        }
        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            CategoryOnlyCallback = categoryOnlyCallback;
            ProcessDirectorUpdateHook = Svc.Hook.HookFromAddress<ProcessDirectorUpdate>(ptr, ProcessDirectorUpdateDetour_Category);
            Enable();
            PluginLog.Information($"Requested Director Update hook and successfully initialized with CATEGORY ONLY data");
        }
        else
        {
            PluginLog.Error($"Could not find DirectorUpdate signature");
        }
    }

    public static void Enable()
    {
        if(ProcessDirectorUpdateHook?.IsEnabled == false) ProcessDirectorUpdateHook?.Enable();
    }

    public static void Disable()
    {
        if(ProcessDirectorUpdateHook?.IsEnabled == true) ProcessDirectorUpdateHook?.Disable();
    }

    public static void Dispose()
    {
        if(ProcessDirectorUpdateHook != null)
        {
            PluginLog.Information($"Disposing Director Update Hook");
            Disable();
            ProcessDirectorUpdateHook?.Dispose();
            ProcessDirectorUpdateHook = null;
        }
    }
}
