using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using Dalamud;
using ECommons.DalamudServices;
using System;
using ECommons.DalamudServices.Legacy;
using ECommons.Logging;
using Dalamud.Game.ClientState.Conditions;

namespace ECommons.Automation;
#nullable disable

/// <summary>
/// Provides automatic cutscene skipping trigger. Does not includes cutscene skipping confirmation.
/// </summary>
public unsafe class AutoCutsceneSkipper
{
    delegate byte CutsceneHandleInputDelegate(nint a1, float a2);
    //[Signature("40 53 48 83 EC 20 80 79 29 00 48 8B D9 0F 85", DetourName = nameof(CutsceneHandleInputDetour))]
    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 80 79 27 00", DetourName = nameof(CutsceneHandleInputDetour))]
    static Hook<CutsceneHandleInputDelegate> CutsceneHandleInputHook;

    //static readonly string ConditionSig = "75 11 BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 84 C0 74 52";
    static readonly string ConditionSig = "75 11 BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 84 C0 74 4C";
    static int ConditionOriginalValuesLen => ConditionSig.Split(" ").Length;
    static nint ConditionAddr;
    /// <summary>
    /// Condition which will be checked to determine if the cutscene should be skipped. Can be null to skip everything unconditionally.
    /// </summary>
    public static Func<nint, bool> Condition;

    /// <summary>
    /// Initializes cutscene skipper trigger. 
    /// </summary>
    /// <param name="cutsceneSkipCondition">Condition which will be checked to determine if the cutscene should be skipped. Can be null to skip everything unconditionally.</param>
    /// <exception cref="Exception">If already initialized</exception>
    public static void Init(Func<nint, bool> cutsceneSkipCondition)
    {
        if (CutsceneHandleInputHook != null) throw new Exception($"{nameof(AutoCutsceneSkipper)} module is already initialized!");
        PluginLog.Information($"AutoCutsceneSkipper requested");
        Condition = cutsceneSkipCondition;
        SignatureHelper.Initialise(new AutoCutsceneSkipper());
        ConditionAddr = Svc.SigScanner.ScanText(ConditionSig);
        PluginLog.Information($"Found cutscene skip condition address at 0x{ConditionAddr:X16}");
        CutsceneHandleInputHook?.Enable();
        PluginLog.Information($"AutoCutsceneSkipper initialized");
    }

    /// <summary>
    /// Disables cutscene skipper trigger. Note that you do not need to call this in Dispose of the plugin, it is disposed automatically.
    /// </summary>
    public static void Disable() => CutsceneHandleInputHook.Disable();
    /// <summary>
    /// Enables previously disabled cutscene trigger. Note that you do not have to call this in constructor of the plugin, it is enabled automatically.
    /// </summary>
    public static void Enable() => CutsceneHandleInputHook.Enable();

    internal static void Dispose()
    {
        CutsceneHandleInputHook?.Dispose();
    }

    internal static byte CutsceneHandleInputDetour(nint a1, float a2)
    {
        if (!Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent])
        {
            return CutsceneHandleInputHook.Original(a1, a2);
        }
        var called = false;
        byte ret = 0;
        try
        {
            if (Condition?.Invoke(a1) != false)
            {
                var skippable = *(nint*)(a1 + 56) != 0;
                if (skippable)
                {
                    SafeMemory.WriteBytes(ConditionAddr, [0xEB]);
                    ret = CutsceneHandleInputHook.Original(a1, a2);
                    called = true;
                    SafeMemory.WriteBytes(ConditionAddr, [0x75]);
                }
            }
        }
        catch (Exception e)
        {
            e.Log();
        }
        if (!called)
        {
            ret = CutsceneHandleInputHook.Original(a1, a2);
        }
        return ret;
    }
}
