using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using ECommons.EzHookManager;
using ECommons.Logging;
using System;

namespace ECommons.Automation;
#nullable disable

/// <summary>
/// Provides automatic cutscene skipping trigger. Does not includes cutscene skipping confirmation.
/// </summary>
public unsafe class AutoCutsceneSkipper
{
    private delegate byte CutsceneHandleInputDelegate(nint a1, float a2);
    private static EzHook<CutsceneHandleInputDelegate> CutsceneHandleInputHook;
    private static EzPatch ConditionPatch;

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
        try
        {
            if(CutsceneHandleInputHook != null) throw new Exception($"{nameof(AutoCutsceneSkipper)} module is already initialized!");
            CutsceneHandleInputHook = new("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 80 79 29 00", CutsceneHandleInputDetour);
            PluginLog.Information($"AutoCutsceneSkipper requested");
            Condition = cutsceneSkipCondition;
            ConditionPatch = new("75 11 BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 84 C0 74 4C", 0, new("EB"), false);
            CutsceneHandleInputHook?.Enable();
            PluginLog.Information($"AutoCutsceneSkipper initialized");
        }
        catch(Exception e)
        {
            e.Log();
        }
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
        //
    }

    internal static byte CutsceneHandleInputDetour(nint a1, float a2)
    {
        if(!Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent])
        {
            return CutsceneHandleInputHook.Original(a1, a2);
        }
        var called = false;
        byte ret = 0;
        try
        {
            if(Condition?.Invoke(a1) != false)
            {
                var skippable = *(nint*)(a1 + 56) != 0;
                if(skippable)
                {
                    ConditionPatch.Enable();
                    ret = CutsceneHandleInputHook.Original(a1, a2);
                    called = true;
                    ConditionPatch.Disable();
                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
        if(!called)
        {
            ret = CutsceneHandleInputHook.Original(a1, a2);
        }
        return ret;
    }
}
