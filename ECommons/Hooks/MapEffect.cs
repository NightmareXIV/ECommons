using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.Logging;
using System;

namespace ECommons.Hooks;
#nullable disable

public static class MapEffect
{
    public const string Sig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B FA 41 0F B7 E8";

    public delegate long ProcessMapEffect(long a1, uint a2, ushort a3, ushort a4);
    internal static Hook<ProcessMapEffect> ProcessMapEffectHook = null;
    static Action<long, uint, ushort, ushort> Callback = null;

    static ProcessMapEffect OriginalDelegate;
    public static ProcessMapEffect Delegate
    {
        get
        {
            if (ProcessMapEffectHook != null && !ProcessMapEffectHook.IsDisposed)
            {
                return ProcessMapEffectHook.Original;
            }
            else
            {
                OriginalDelegate ??= EzDelegate.Get<ProcessMapEffect>(Sig);
                return OriginalDelegate;
            }
        }
    }

    internal static long ProcessMapEffectDetour(long a1, uint a2, ushort a3, ushort a4)
    {
        try
        {
            Callback(a1, a2, a3, a4);
        }
        catch (Exception e)
        {
            e.Log();
        }
        return ProcessMapEffectHook.Original(a1, a2, a3, a4);
    }

    public static void Init(Action<long, uint, ushort, ushort> fullParamsCallback)
    {
        if (ProcessMapEffectHook != null)
        {
            throw new Exception("MapEffect is already initialized!");
        }
        if (Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            Callback = fullParamsCallback;
            ProcessMapEffectHook = Svc.Hook.HookFromAddress<ProcessMapEffect>(ptr, ProcessMapEffectDetour);
            Enable();
            PluginLog.Information($"Requested MapEffect hook and successfully initialized");
        }
        else
        {
            PluginLog.Error($"Could not find MapEffect signature");
        }
    }

    public static void Enable()
    {
        if (ProcessMapEffectHook?.IsEnabled == false) ProcessMapEffectHook?.Enable();
    }

    public static void Disable()
    {
        if (ProcessMapEffectHook?.IsEnabled == true) ProcessMapEffectHook?.Disable();
    }

    public static void Dispose()
    {
        if (ProcessMapEffectHook != null)
        {
            PluginLog.Information($"Disposing MapEffect Hook");
            Disable();
            ProcessMapEffectHook?.Dispose();
            ProcessMapEffectHook = null;
        }
    }
}
