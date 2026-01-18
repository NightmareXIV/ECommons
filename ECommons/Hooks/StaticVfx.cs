#nullable disable

#region

using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;

#endregion

namespace ECommons.Hooks;

public static class StaticVfx
{
    /// <summary>
    ///     The signature your method must match to subscribe to
    ///     <see cref="StaticVfxCreateEvent" />.
    /// </summary>
    /// <param name="vfxPtr">
    ///     Pointer to the newly-created Static VFX instance, this is the
    ///     <see langword="return" /> from calling <c>.Original()</c> inside
    ///     of that Detour, prior to calling your Delegate.<br />
    ///     Due to that, this delegate does NOT match the signature of the
    ///     Detour itself.<br />
    ///     Can be cast to a <see cref="ECommons.GameHelpers.VfxStruct" />.
    /// </param>
    /// <param name="path">The VFX path string.</param>
    /// <param name="systemSource">
    ///     The FFXIV subsystem that is responsible for creating the VFX.
    /// </param>
    /// <remarks>
    ///     WARNING! Do NOT try to call <c>.Original()</c> within your delegate!
    ///     <br />
    ///     These delegates are already called after that is run (to provide
    ///     <paramref name="vfxPtr" />).
    /// </remarks>
    public delegate void StaticVfxCreateCallbackDelegate(nint vfxPtr, string path,
        string systemSource);

    /// <summary>
    ///     The signature your method must match to subscribe to
    ///     <see cref="StaticVfxDtorEvent" />.
    /// </summary>
    /// <param name="staticVfxAddress">
    ///     Address of the Static VFX instance being destructed.<br />
    ///     Can be cast to a <see cref="ECommons.GameHelpers.VfxStruct" />.
    /// </param>
    /// <remarks>
    ///     WARNING! Do NOT try to call <c>.Original()</c> within your delegate!
    ///     <br />
    ///     These delegates are already called right before that is run.
    /// </remarks>
    public delegate void StaticVfxDtorCallbackDelegate(nint staticVfxAddress);

    /// <summary>
    ///     The signature your method must match to subscribe to
    ///     <see cref="StaticVfxRunEvent" />.
    /// </summary>
    /// <param name="staticVfxAddress">
    ///     Address of the Static VFX instance being run.<br />
    ///     Can be cast to a <see cref="ECommons.GameHelpers.VfxStruct" />.
    /// </param>
    /// <param name="a1">
    ///     Unknown float(?) parameter.<br />
    ///     Possibly related to position/rotation?
    /// </param>
    /// <param name="a2">
    ///     Unknown uint(?) parameter.<br />
    ///     Possibly related to position/rotation?<br />
    ///     Possibly an incorrectly-typed game object id?
    /// </param>
    /// <remarks>
    ///     WARNING! Do NOT try to call <c>.Original()</c> within your delegate!
    ///     <br />
    ///     These delegates are already called right before that is run.
    /// </remarks>
    public delegate void StaticVfxRunCallbackDelegate(nint staticVfxAddress,
        float a1, uint a2);

    public static readonly string CreateSig = "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08";

    public static readonly string RunSig = "E8 ?? ?? ?? ?? B0 02 EB 02";

    public static readonly string DtorSig = "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9";

    private static Hook<StaticVfxCreateDelegate>? StaticVfxCreateHook;

    private static Hook<StaticVfxRunDelegate>? StaticVfxRunHook;

    private static Hook<StaticVfxDtorDelegate>? StaticVfxDtorHook;

    private static event StaticVfxCreateCallbackDelegate? _staticVfxCreateEvent;

    private static event StaticVfxRunCallbackDelegate? _staticVfxRunEvent;

    private static event StaticVfxDtorCallbackDelegate? _staticVfxDtorEvent;

    /// <summary>
    ///     Add a <see cref="StaticVfxCreateCallbackDelegate" /> subscriber
    ///     to this event to be called when a static VFX is created.
    /// </summary>
    public static event StaticVfxCreateCallbackDelegate StaticVfxCreateEvent
    {
        add
        {
            HookCreate();
            _staticVfxCreateEvent += value;
        }
        remove => _staticVfxCreateEvent -= value;
    }

    /// <summary>
    ///     Add a <see cref="StaticVfxRunCallbackDelegate" /> subscriber
    ///     to this event to be called when a static VFX is run.
    /// </summary>
    public static event StaticVfxRunCallbackDelegate StaticVfxRunEvent
    {
        add
        {
            HookRun();
            _staticVfxRunEvent += value;
        }
        remove => _staticVfxRunEvent -= value;
    }

    /// <summary>
    ///     Add a <see cref="StaticVfxDtorCallbackDelegate" /> subscriber
    ///     to this event to be called when a static VFX is destructed.
    /// </summary>
    public static event StaticVfxDtorCallbackDelegate StaticVfxDtorEvent
    {
        add
        {
            HookDtor();
            _staticVfxDtorEvent += value;
        }
        remove => _staticVfxDtorEvent -= value;
    }

    internal static nint StaticVfxCreateDetour(string a1, string a2)
    {
        var output = StaticVfxCreateHook!.Original(a1, a2);

        try
        {
            var @event = _staticVfxCreateEvent;
            if(@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach(var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod =
                            (StaticVfxCreateCallbackDelegate)subscriber;
                        subscriberMethod(output, a1, a2);
                    }
                    catch(Exception e)
                    {
                        e.Log();
                    }
                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }

        return output;
    }

    internal static nint StaticVfxRunDetour(nint a1, float a2, uint a3)
    {
        try
        {
            var @event = _staticVfxRunEvent;
            if(@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach(var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod =
                            (StaticVfxRunCallbackDelegate)subscriber;
                        subscriberMethod(a1, a2, a3);
                    }
                    catch(Exception e)
                    {
                        e.Log();
                    }
                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }

        return StaticVfxRunHook!.Original(a1, a2, a3);
    }

    internal static void StaticVfxDtorDetour(nint a1)
    {
        try
        {
            var @event = _staticVfxDtorEvent;
            if(@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach(var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod =
                            (StaticVfxDtorCallbackDelegate)subscriber;
                        subscriberMethod(a1);
                    }
                    catch(Exception e)
                    {
                        e.Log();
                    }
                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }

        StaticVfxDtorHook!.Original(a1);
    }

    private static void HookCreate()
    {
        if(StaticVfxCreateHook != null)
            return;

        if(Svc.SigScanner.TryScanText(CreateSig, out var ptr))
        {
            StaticVfxCreateHook =
                Svc.Hook.HookFromAddress<StaticVfxCreateDelegate>(ptr,
                    StaticVfxCreateDetour);
            StaticVfxCreateHook.Enable();
            PluginLog.Information(
                "Requested Static Vfx Create hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Static Vfx Create signature");
        }
    }

    private static void HookRun()
    {
        if(StaticVfxRunHook != null)
            return;

        if(Svc.SigScanner.TryScanText(RunSig, out var ptr))
        {
            StaticVfxRunHook =
                Svc.Hook.HookFromAddress<StaticVfxRunDelegate>(ptr,
                    StaticVfxRunDetour);
            StaticVfxRunHook.Enable();
            PluginLog.Information(
                "Requested Static Vfx Run hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Static Vfx Run signature");
        }
    }

    private static void HookDtor()
    {
        if(StaticVfxDtorHook != null)
            return;

        if(Svc.SigScanner.TryScanText(DtorSig, out var ptr))
        {
            StaticVfxDtorHook =
                Svc.Hook.HookFromAddress<StaticVfxDtorDelegate>(ptr,
                    StaticVfxDtorDetour);
            StaticVfxDtorHook.Enable();
            PluginLog.Information(
                "Requested Static Vfx Dtor hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Static Vfx Dtor signature");
        }
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="StaticVfxCreateEvent" />.
    /// </remarks>
    public static void EnableCreate()
    {
        if(StaticVfxCreateHook?.IsEnabled == false)
            StaticVfxCreateHook?.Enable();
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="StaticVfxRunEvent" />.
    /// </remarks>
    public static void EnableRun()
    {
        if(StaticVfxRunHook?.IsEnabled == false)
            StaticVfxRunHook?.Enable();
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="StaticVfxDtorEvent" />.
    /// </remarks>
    public static void EnableDtor()
    {
        if(StaticVfxDtorHook?.IsEnabled == false)
            StaticVfxDtorHook?.Enable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void DisableCreate()
    {
        if(StaticVfxCreateHook?.IsEnabled == true)
            StaticVfxCreateHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void DisableRun()
    {
        if(StaticVfxRunHook?.IsEnabled == true)
            StaticVfxRunHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void DisableDtor()
    {
        if(StaticVfxDtorHook?.IsEnabled == true)
            StaticVfxDtorHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="ECommons.ECommonsMain.Dispose()" />.
    /// </remarks>
    public static void Dispose()
    {
        if(StaticVfxCreateHook != null)
        {
            PluginLog.Information("Disposing StaticVfx Create Hook");
            DisableCreate();
            if(!StaticVfxCreateHook.IsDisposed)
                StaticVfxCreateHook?.Dispose();
            StaticVfxCreateHook = null;
        }

        if(StaticVfxRunHook != null)
        {
            PluginLog.Information("Disposing StaticVfx Run Hook");
            DisableRun();
            if(!StaticVfxRunHook.IsDisposed)
                StaticVfxRunHook?.Dispose();
            StaticVfxRunHook = null;
        }

        if(StaticVfxDtorHook != null)
        {
            PluginLog.Information("Disposing StaticVfx Dtor Hook");
            DisableDtor();
            if(!StaticVfxDtorHook.IsDisposed)
                StaticVfxDtorHook?.Dispose();
            StaticVfxDtorHook = null;
        }
    }

    private delegate nint StaticVfxCreateDelegate(string a1, string a2);

    private delegate nint StaticVfxRunDelegate(nint a1, float a2, uint a3);

    private delegate void StaticVfxDtorDelegate(nint a1);
}