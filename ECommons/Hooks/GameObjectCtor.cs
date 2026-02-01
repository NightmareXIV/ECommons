#nullable disable

#region

using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.Schedulers;
using System;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace ECommons.Hooks;

public class GameObjectCtor
{
    public delegate void GameObjectConstructorCallbackDelegate(nint objectAddress);

    public const string Sig =
        "48 8D 05 ?? ?? ?? ?? C7 81 ?? ?? ?? ?? ?? ?? ?? ?? 48 89 01 48 8B C1 C3";

    private static Hook<GameObjectConstructorDelegate> GameObjectConstructorHook;

    private static event GameObjectConstructorCallbackDelegate
        _gameObjectConstructorEvent;

    /// <summary>
    ///     Add a <see cref="GameObjectConstructorCallbackDelegate" /> subscriber
    ///     to this event be called when a Game Object is created.
    /// </summary>
    public static event GameObjectConstructorCallbackDelegate
        GameObjectConstructorEvent
        {
            add
            {
                Hook();
                _gameObjectConstructorEvent += value;
            }
            remove => _gameObjectConstructorEvent -= value;
        }

    internal static nint GameObjectConstructorDetour(nint a1)
    {
        try
        {
            Svc.Framework.RunOnTick(run, delayTicks:1);
            Task run()
            {
                var @event = _gameObjectConstructorEvent;
                if(@event != null)
                {
                    var obj = Svc.Objects.FirstOrDefault(x => x.Address == a1);
                    if(obj != null)
                    {
                        // Iterate individual subscribers so a failing subscriber doesn't stop the rest.
                        foreach(var subscriber in @event.GetInvocationList())
                        {
                            try
                            {
                                var subscriberMethod = (GameObjectConstructorCallbackDelegate)subscriber;
                                subscriberMethod(a1);
                            }
                            catch(Exception e)
                            {
                                e.Log();
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        }
        catch(Exception e)
        {
            e.Log();
        }

        return GameObjectConstructorHook!.Original(a1);
    }

    private static void Hook()
    {
        if(GameObjectConstructorHook != null)
            return;

        if(Svc.SigScanner.TryScanText(Sig, out var ptr))
        {
            GameObjectConstructorHook =
                Svc.Hook.HookFromAddress<GameObjectConstructorDelegate>(ptr,
                    GameObjectConstructorDetour);
            Enable();
            PluginLog.Information(
                "Requested Game Object ctor() hook and successfully initialized");
        }
        else
        {
            PluginLog.Error("Could not find Game Object ctor() signature");
        }
    }

    /// <remarks>
    ///     Already called when you subscribe a delegate to
    ///     <see cref="GameObjectConstructorEvent" />.
    /// </remarks>
    public static void Enable()
    {
        if(GameObjectConstructorHook?.IsEnabled == false)
            GameObjectConstructorHook?.Enable();
    }

    /// <remarks>
    ///     Already called in <see cref="Dispose()" />.
    /// </remarks>
    public static void Disable()
    {
        if(GameObjectConstructorHook?.IsEnabled == true)
            GameObjectConstructorHook?.Disable();
    }

    /// <remarks>
    ///     Already called in <see cref="ECommons.ECommonsMain.Dispose()" />.
    /// </remarks>
    public static void Dispose()
    {
        if(GameObjectConstructorHook == null)
            return;

        PluginLog.Information("Disposing Game Object ctor() Hook");
        Disable();
        if(!GameObjectConstructorHook.IsDisposed)
            GameObjectConstructorHook?.Dispose();
        GameObjectConstructorHook = null;
    }

    private delegate nint GameObjectConstructorDelegate(nint a1);
}