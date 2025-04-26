using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.Schedulers;
using System;
using System.Collections.Generic;

namespace ECommons.ObjectLifeTracker;
#nullable disable

public static class ObjectLife
{
    private delegate IntPtr IGameObject_ctor(IntPtr obj);

    private static Hook<IGameObject_ctor> IGameObject_ctor_hook = null;
    private static Dictionary<IntPtr, long> IGameObjectLifeTime = null;
    public static Action<nint> OnObjectCreation = null;

    internal static void Init()
    {
        new TickScheduler(() =>
        {
            IGameObjectLifeTime = [];
#pragma warning disable CS0618 // Type or member is obsolete
            IGameObject_ctor_hook = Svc.Hook.HookFromAddress<IGameObject_ctor>(Svc.SigScanner.ScanText("48 8D 05 ?? ?? ?? ?? C7 81 ?? ?? ?? ?? ?? ?? ?? ?? 48 89 01 48 8B C1 C3"), IGameObject_ctor_detour);
#pragma warning restore CS0618 // Type or member is obsolete
            IGameObject_ctor_hook.Enable();
            foreach(var x in Svc.Objects)
            {
                IGameObjectLifeTime[x.Address] = Environment.TickCount64;
            }
        });
    }

    internal static void Dispose()
    {
        if(IGameObject_ctor_hook != null)
        {
            IGameObject_ctor_hook.Disable();
            IGameObject_ctor_hook.Dispose();
            IGameObject_ctor_hook = null;
        }
        IGameObjectLifeTime = null;
    }

    private static IntPtr IGameObject_ctor_detour(IntPtr ptr)
    {
        if(IGameObjectLifeTime == null)
        {
            throw new Exception("IGameObjectLifeTime is null. Have you initialised the ObjectLife module on ECommons initialisation?");
        }
        IGameObjectLifeTime[ptr] = Environment.TickCount64;
        var ret = IGameObject_ctor_hook.Original(ptr);

        if(OnObjectCreation != null)
        {
            try
            {
                OnObjectCreation(ptr);
            }
            catch(Exception e)
            {
                e.Log($"Exception in IGameObject_ctor_detour");
            }
        }
        return ret;
    }

    public static long GetLifeTime(this IGameObject o)
    {
        return Environment.TickCount64 - GetSpawnTime(o);
    }

    public static float GetLifeTimeSeconds(this IGameObject o)
    {
        return (float)o.GetLifeTime() / 1000f;
    }

    public static long GetSpawnTime(this IGameObject o)
    {
        if(IGameObject_ctor_hook == null) throw new Exception("Object life tracker was not initialized");
        if(IGameObjectLifeTime.TryGetValue(o.Address, out var result))
        {
            return result;
        }
        else
        {
            PluginLog.Warning($"Warning: object life data could not be found\n" +
                $"Object addr: {o.Address:X16} ID: {o.EntityId:X8} Name: {o.Name}");
            return 0;
        }
    }
}
