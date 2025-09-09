using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;

namespace ECommons.Hooks;
#nullable disable

public static unsafe class MapEffect
{
    public const string Sig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B FA 41 0F B7 E8";
    public const string ExsMultisig = "40 55 41 57 48 83 EC ?? 48 83 B9";

    public delegate long ProcessMapEffect(long a1, uint a2, ushort a3, ushort a4);
    internal static Hook<ProcessMapEffect> ProcessMapEffectHook = null;
    private static Action<long, uint, ushort, ushort> Callback = null;
    private static ProcessMapEffect OriginalDelegate;

    private static class Ex
    {
        private delegate void ProcessMapEffectEx(ContentDirector* director, byte* packetPtr);
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx0Hook = null;
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx1Hook = null;
        private static EzHook<ProcessMapEffectEx> ProcessMapEffectEx2Hook = null;

        internal static void Initialize()
        {
            var result = Svc.SigScanner.ScanAllText(ExsMultisig);
            if(result.Length != 3)
            {
                PluginLog.Warning($"Failed to initialize ProcessMapEffectEx. Expected 3 results, found {result.Length}.");
            }
            else
            {
                ProcessMapEffectEx0Hook = new(result[0], ProcessMapEffectEx0Detour);
                ProcessMapEffectEx1Hook = new(result[1], ProcessMapEffectEx1Detour);
                ProcessMapEffectEx2Hook = new(result[2], ProcessMapEffectEx2Detour);
            }
        }

        private static void ProcessMapEffectEx0Detour(ContentDirector* director, byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 10, 18);
            ProcessMapEffectEx0Hook.Original(director, packetPtr);
        }
        private static void ProcessMapEffectEx1Detour(ContentDirector* director, byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 18, 34);
            ProcessMapEffectEx1Hook.Original(director, packetPtr);
        }
        private static void ProcessMapEffectEx2Detour(ContentDirector* director, byte* packetPtr)
        {
            ProcessMapEffectExCommon(packetPtr, 26, 50);
            ProcessMapEffectEx2Hook.Original(director, packetPtr);
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
