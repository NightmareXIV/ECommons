using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.Logging;
using System;

namespace ECommons.Hooks;
#nullable disable

public static unsafe class MapEffect
{
    public const string Sig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B FA 41 0F B7 E8";
    public const string ExSig = "E8 ?? ?? ?? ?? B0 01 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 83 C4 50 5F C3 48 8B CB E8 ?? ?? ?? ?? B0 01 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 83 C4 50 5F C3 0F B6 43 1E";

    public delegate long ProcessMapEffect(long a1, uint a2, ushort a3, ushort a4);
    internal static Hook<ProcessMapEffect> ProcessMapEffectHook = null;
    private static Action<long, uint, ushort, ushort> Callback = null;
    private static ProcessMapEffect OriginalDelegate;

    private static class Ex
    {
        private delegate void ProcessMapEffectEx(byte* packetPtr);
        [EzHook(ExSig)]
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx0Hook = null;
        [EzHook(ExSig, 0x40)]
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx1Hook = null;
        [EzHook(ExSig, 0x80)]
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx2Hook = null;

        internal static void Initialize()
        {
            EzSignatureHelper.Initialize(typeof(Ex));
        }

        private static void ProcessMapEffectEx0Detour(byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 10, 18);
            ProcessMapEffectEx0Hook.Original(packetPtr);
        }
        private static void ProcessMapEffectEx1Detour(byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 18, 34);
            ProcessMapEffectEx1Hook.Original(packetPtr);
        }
        private static void ProcessMapEffectEx2Detour(byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 26, 50);
            ProcessMapEffectEx2Hook.Original(packetPtr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetPtr"></param>
        /// <param name="offset1">v5</param>
        /// <param name="offset2">v8</param>
        private static void ProcessMapEffectExCommon(byte* packetPtr, int offset1, int offset2)
        {
            try
            {
                for(int i = 0; i < packetPtr[0]; i++)
                {
                    var a2 = packetPtr[i + offset2];
                    var a3 = *(ushort*)&packetPtr[2 * i + 2];
                    var a4 = *(ushort*)&packetPtr[2 * i + offset1];
                    PluginLog.Debug($"Map Effect Ex Hook: {a2}/{a3}/{a4} at {offset1}, {offset2}, {i}");
                    Callback(0, a2, a3, a4);
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
    }

    public static ProcessMapEffect Delegate
    {
        get
        {
            if(ProcessMapEffectHook != null && !ProcessMapEffectHook.IsDisposed)
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
        catch(Exception e)
        {
            e.Log();
        }
        return ProcessMapEffectHook.Original(a1, a2, a3, a4);
    }

    public static void Init(Action<long, uint, ushort, ushort> fullParamsCallback)
    {
        if(ProcessMapEffectHook != null)
        {
            throw new Exception("MapEffect is already initialized!");
        }
        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            Callback = fullParamsCallback;
            ProcessMapEffectHook = Svc.Hook.HookFromAddress<ProcessMapEffect>(ptr, ProcessMapEffectDetour);
            Enable();
            PluginLog.Information($"Requested MapEffect hook and successfully initialized");
            Ex.Initialize();
        }
        else
        {
            PluginLog.Error($"Could not find MapEffect signature");
        }
    }

    public static void Enable()
    {
        if(ProcessMapEffectHook?.IsEnabled == false) ProcessMapEffectHook?.Enable();
    }

    public static void Disable()
    {
        if(ProcessMapEffectHook?.IsEnabled == true) ProcessMapEffectHook?.Disable();
    }

    public static void Dispose()
    {
        if(ProcessMapEffectHook != null)
        {
            PluginLog.Information($"Disposing MapEffect Hook");
            Disable();
            ProcessMapEffectHook?.Dispose();
            ProcessMapEffectHook = null;
        }
    }
}
