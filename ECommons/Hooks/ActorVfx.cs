#nullable disable

#region

using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;

#endregion

namespace ECommons.Hooks;

public static unsafe class ActorVfx
{
    /// <summary>
    ///     The signature your method must match to subscribe to
    ///     <see cref="ActorVfxCreateEvent" />.
    /// </summary>
    /// <param name="vfxPtr">
    ///     Pointer to the newly-created Actor VFX instance, this is the
    ///     <see langword="return" /> from calling <c>.Original()</c> inside
    ///     of that Detour, prior to calling your Delegate.<br />
    ///     Due to that, this delegate does NOT match the signature of the
    ///     Detour itself.<br />
    ///     Can be cast to a <see cref="ECommons.GameHelpers.VfxStruct" />.
    /// </param>
    /// <param name="vfxPathPtr">
    ///     Pointer to the VFX path string.<br />
    ///     Can resolve to a string with
    ///     <see
    ///         cref="Dalamud.Memory.MemoryHelper.ReadString(nint, System.Text.Encoding, int)">
    ///         MemoryHelper.ReadString((nint)vfxPathPtr, Encoding.ASCII, 256)
    ///     </see>
    ///     .
    /// </param>
    /// <param name="casterAddress">Address of the caster GameObject.</param>
    /// <param name="targetAddress">Address of the target GameObject.</param>
    /// <param name="a4">Unknown float parameter.</param>
    /// <param name="a5">Unknown char parameter.</param>
    /// <param name="a6">Unknown ushort parameter.</param>
    /// <param name="a7">Unknown char parameter.</param>
    /// <remarks>
    ///     WARNING! Do NOT try to call <c>.Original()</c> within your delegate!
    ///     <br />
    ///     These delegates are called after that is already run (to provide
    ///     <paramref name="vfxPtr" />).
    /// </remarks>
    public delegate void ActorVfxCreateCallbackDelegate(nint vfxPtr, nint vfxPathPtr, nint casterAddress, nint targetAddress, float a4, byte a5, ushort a6, byte a7);

    /// <summary>
    ///     The signature your method must match to subscribe to
    ///     <see cref="ActorVfxDtorEvent" />.
    /// </summary>
    /// <param name="actorVfxAddress">
    ///     Address of the Actor VFX instance being destructed.<br />
    ///     Can be cast to a <see cref="ECommons.GameHelpers.VfxStruct" />.
    /// </param>
    /// <remarks>
    ///     WARNING! Do NOT try to call <c>.Original()</c> within your delegate!
    ///     <br />
    ///     These delegates are already called right before that is run.
    /// </remarks>
    public delegate void ActorVfxDtorCallbackDelegate(nint actorVfxAddress);

    public static readonly string CreateSig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";

    public static readonly string DtorSig = "48 89 5C 24 ?? 57 48 83 EC ?? 48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 8B FA 48 8D 05 ?? ?? ?? ?? 48 89 81 ?? ?? ?? ?? 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 ?? 48 8B 01 48 8B D3";

    private static Hook<ActorVfxCreateDelegate> ActorVfxCreateHook;

    private static Hook<ActorVfxDtorDelegate> ActorVfxDtorHook;

    private static event ActorVfxCreateCallbackDelegate _actorVfxCreateEvent;

    private static event ActorVfxDtorCallbackDelegate _actorVfxDtorEvent;

    /// <summary>
    ///     Add a <see cref="ActorVfxCreateCallbackDelegate" /> subscriber
    ///     to this event be called when an actor VFX is created.
    /// </summary>
    public static event ActorVfxCreateCallbackDelegate ActorVfxCreateEvent
    {
        add
        {
            HookCreate();
            _actorVfxCreateEvent += value;
        }
        remove => _actorVfxCreateEvent -= value;
    }

    /// <summary>
    ///     Add a <see cref="ActorVfxDtorCallbackDelegate" /> subscriber
    ///     to this event be called when an actor VFX is destructed.
    /// </summary>
    public static event ActorVfxDtorCallbackDelegate ActorVfxDtorEvent
    {
        add
        {
            HookDtor();
            _actorVfxDtorEvent += value;
        }
        remove => _actorVfxDtorEvent -= value;
    }

    internal static nint ActorVfxCreateDetour(nint a1, nint a2, nint a3, float a4, byte a5, ushort a6, byte a7)
    {
        var output = ActorVfxCreateHook!.Original(a1, a2, a3, a4, a5, a6, a7);

        try
        {
            var @event = _actorVfxCreateEvent;
            if(@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach(var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod = (ActorVfxCreateCallbackDelegate)subscriber;
                        subscriberMethod(output, a1, a2, a3, a4, a5, a6, a7);
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

    internal static void ActorVfxDtorDetour(nint a1)
    {
        try
        {
            var @event = _actorVfxDtorEvent;
            if(@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach(var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod =
                            (ActorVfxDtorCallbackDelegate)subscriber;
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

        ActorVfxDtorHook!.Original(a1);
    }

    private static void HookCreate()
    {
        if(ActorVfxCreateHook != null)
            return;

        if(Svc.SigScanner.TryScanText(CreateSig, out var ptr))
        {
            ActorVfxCreateHook = Svc.Hook.HookFromAddress<ActorVfxCreateDelegate>(ptr, ActorVfxCreateDetour);
            EnableCreate();
            PluginLog.Information("Requested Actor Vfx Create hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Actor Vfx Create signature");
        }
    }

    private static void HookDtor()
    {
        if(ActorVfxDtorHook != null)
            return;

        if(Svc.SigScanner.TryScanText(DtorSig, out var ptr))
        {
            ActorVfxDtorHook =
                Svc.Hook.HookFromAddress<ActorVfxDtorDelegate>(ptr,
                    ActorVfxDtorDetour);
            EnableDtor();
            PluginLog.Information(
                "Requested Actor Vfx Dtor hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Actor Vfx Dtor signature");
        }
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="ActorVfxCreateEvent" />.
    /// </remarks>
    public static void EnableCreate()
    {
        if(ActorVfxCreateHook?.IsEnabled == false)
            ActorVfxCreateHook?.Enable();
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="ActorVfxDtorEvent" />.
    /// </remarks>
    public static void EnableDtor()
    {
        if(ActorVfxDtorHook?.IsEnabled == false)
            ActorVfxDtorHook?.Enable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void DisableCreate()
    {
        if(ActorVfxCreateHook?.IsEnabled == true)
            ActorVfxCreateHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void DisableDtor()
    {
        if(ActorVfxDtorHook?.IsEnabled == true)
            ActorVfxDtorHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="ECommons.ECommonsMain.Dispose()" />.
    /// </remarks>
    public static void Dispose()
    {
        if(ActorVfxCreateHook != null)
        {
            PluginLog.Information("Disposing ActorVfx Create Hook");
            DisableCreate();
            if(!ActorVfxCreateHook.IsDisposed)
                ActorVfxCreateHook?.Dispose();
            ActorVfxCreateHook = null;
        }

        if(ActorVfxDtorHook != null)
        {
            PluginLog.Information("Disposing ActorVfx Dtor Hook");
            DisableDtor();
            if(!ActorVfxDtorHook.IsDisposed)
                ActorVfxDtorHook?.Dispose();
            ActorVfxDtorHook = null;
        }
    }

    private delegate nint ActorVfxCreateDelegate(nint a1, nint a2, nint a3, float a4, byte a5, ushort a6, byte a7);

    private delegate void ActorVfxDtorDelegate(nint a1);
}