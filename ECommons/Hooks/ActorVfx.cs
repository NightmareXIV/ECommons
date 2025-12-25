using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;

#nullable disable

namespace ECommons.Hooks;

public static unsafe class ActorVfx
{
    public const string Sig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";

    private delegate nint ActorVfxCreateDelegate(char* a1, nint a2, nint a3, float a4, char a5, ushort a6, char a7);
    public delegate nint ActorVfxCreateCallbackDelegate(char* a1, nint a2, nint a3, float a4, char a5, ushort a6, char a7);

    private static Hook<ActorVfxCreateDelegate> ActorVfxCreateHook = null;

    private static event ActorVfxCreateCallbackDelegate _actorVfxCreateEvent;
    /// <summary>
    ///     Add a <see cref="ActorVfxCreateCallbackDelegate"/> subscriber
    ///     to this event be called when an actor VFX is created.
    /// </summary>
    public static event ActorVfxCreateCallbackDelegate ActorVfxCreateEvent
    {
        add
        {
            Hook();
            _actorVfxCreateEvent += value;
        }
        remove => _actorVfxCreateEvent -= value;
    }
    
    internal static nint ActorVfxCreateDetour(char* a1, nint a2, nint a3, float a4, char a5, ushort a6, char a7)
    {
        try
        {
            var @event = _actorVfxCreateEvent;
            if (@event != null)
            {
                // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                foreach (var subscriber in @event.GetInvocationList())
                {
                    try
                    {
                        var subscriberMethod =
                            (ActorVfxCreateCallbackDelegate)subscriber;
                        subscriberMethod(a1, a2, a3, a4, a5, a6, a7);
                    }
                    catch (Exception e)
                    {
                        e.Log();
                    }
                }
            }
        }
        catch (Exception e)
        {
            e.Log();
        }
        return ActorVfxCreateHook!.Original(a1, a2, a3, a4, a5, a6, a7);
    }

    private static void Hook()
    {
        if(ActorVfxCreateHook != null)
            return;

        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            ActorVfxCreateHook = Svc.Hook.HookFromAddress<ActorVfxCreateDelegate>(ptr, ActorVfxCreateDetour);
            Enable();
            PluginLog.Information($"Requested Action Effect hook and successfully initialized");
        }
        else
        {
            PluginLog.Error($"Could not find ActionEffect signature");
        }
    }

    public static void Enable()
    {
        if(ActorVfxCreateHook?.IsEnabled == false)
            ActorVfxCreateHook?.Enable();
    }

    public static void Disable()
    {
        if(ActorVfxCreateHook?.IsEnabled == true)
            ActorVfxCreateHook?.Disable();
    }

    public static void Dispose()
    {
        if(ActorVfxCreateHook == null)
            return;

        PluginLog.Information($"Disposing ActorVfx Hook");
        Disable();
        if (!ActorVfxCreateHook.IsDisposed)
            ActorVfxCreateHook?.Dispose();
        ActorVfxCreateHook = null;
    }
}