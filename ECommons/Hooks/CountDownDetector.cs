using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;

namespace ECommons.Hooks;
public unsafe static class CountDownDetector
{
    private static readonly string Sig = "40 53 48 83 EC 40 80 79 38 00";

    public delegate IntPtr CountdownTimerDelegate(ulong* p1);
    private static ulong* _paramPointer = null;
    internal static Hook<CountdownTimerDelegate>? CountdownTimerDelegateHook = null;

    public static bool isEnabled => CountdownTimerDelegateHook?.IsEnabled == true;

    public static void Init()
    {
        if(CountdownTimerDelegateHook != null)
        {
            throw new Exception("Director Update Hook is already initialized!");
        }
        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            CountdownTimerDelegateHook = Svc.Hook.HookFromAddress<CountdownTimerDelegate>(ptr, OnCountDawn);
            Enable();
            PluginLog.Information($"Requested Director Update hook and successfully initialized with FULL data");
        }
        else
        {
            PluginLog.Error($"Could not find DirectorUpdate signature");
        }
    }

    private static IntPtr OnCountDawn(ulong* p1)
    {
        _paramPointer = p1;
        return CountdownTimerDelegateHook!.Original(p1);
    }

    public static void Enable()
    {
        if(CountdownTimerDelegateHook?.IsEnabled == false) CountdownTimerDelegateHook?.Enable();
    }

    public static void Disable()
    {
        if(CountdownTimerDelegateHook?.IsEnabled == true) CountdownTimerDelegateHook?.Disable();
    }

    public static bool IsActiveCountDown()
    {
        if(CountdownTimerDelegateHook == null) return false;
        if(_paramPointer == null) return false;
        return *(bool*)((byte*)_paramPointer + 0x38);
    }

    public static float GetCountDownRemainingTime()
    {
        if(CountdownTimerDelegateHook == null) return 0;
        if(_paramPointer == null) return 0;
        if(!IsActiveCountDown()) return 0;
        return *(float*)((byte*)_paramPointer + 0x2c);
    }

    public static void Dispose()
    {
        if(CountdownTimerDelegateHook != null)
        {
            PluginLog.Information($"Disposing CountDown Hook");
            Disable();
            CountdownTimerDelegateHook?.Dispose();
            CountdownTimerDelegateHook = null;
        }
    }
}
