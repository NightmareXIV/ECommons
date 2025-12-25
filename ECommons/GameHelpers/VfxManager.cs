#region

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.Hooks;
using ECommons.Logging;
using ECommons.Throttlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ECommons.GameHelpers;

public class VfxManager
{
    public static readonly TimeSpan VfxExpiryDuration = TimeSpan.FromSeconds(30);
    
    public static readonly Dictionary<ulong, List<VfxInfo>> TrackedEffects = [];

    // https://github.com/PunishXIV/Splatoon/blob/main/Splatoon/Utility/Utils.cs#L482
    private static readonly string[] BlacklistedVfx =
    [
        "vfx/common/eff/dk04ht_canc0h.avfx",
        "vfx/common/eff/dk02ht_totu0y.avfx",
        "vfx/common/eff/dk05th_stup0t.avfx",
        "vfx/common/eff/dk10ht_wra0c.avfx",
        "vfx/common/eff/cmat_ligct0c.avfx",
        "vfx/common/eff/dk07ht_da00c.avfx",
        "vfx/common/eff/cmat_icect0c.avfx",
        "vfx/common/eff/dk10ht_ice2c.avfx",
        "vfx/common/eff/combo_001f.avfx",
        "vfx/common/eff/dk02ht_da00c.avfx",
        "vfx/common/eff/dk06gd_par0h.avfx",
        "vfx/common/eff/dk04ht_fir0h.avfx",
        "vfx/common/eff/dk05th_stdn0t.avfx",
        "vfx/common/eff/dk06mg_mab0h.avfx",
        "vfx/common/eff/mgc_2kt001c1t.avfx",
    ];

    internal static unsafe void Init()
    {
        ActorVfx.ActorVfxCreateEvent              += TrackOnVfxCreate;
        ActorVfx.ActorVfxDtorEvent                += RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent += EmptyVfxList;
        Svc.ClientState.TerritoryChanged          += EmptyVfxList;
        Svc.Framework.Update                      += EmptyVfxListPeriodically;
    }

    internal static unsafe void Dispose()
    {
        lock (TrackedEffects)
            TrackedEffects.Clear();
        ActorVfx.ActorVfxCreateEvent              -= TrackOnVfxCreate;
        ActorVfx.ActorVfxDtorEvent                -= RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent -= EmptyVfxList;
        Svc.ClientState.TerritoryChanged          -= EmptyVfxList;
        Svc.Framework.Update                      -= EmptyVfxListPeriodically;
    }

    private static unsafe void TrackOnVfxCreate
    (nint vfxPtr, char* vfxPathPtr, nint casterAddress, nint targetAddress,
        float a4, char a5, ushort a6, char a7)
    {
        var spawnTick = Environment.TickCount64;
        ulong casterID;
        ulong targetID;
        string path;
        try
        {
            var casterObject = Svc.Objects.CreateObjectReference(casterAddress);
            var targetObject = Svc.Objects.CreateObjectReference(targetAddress);

            if(casterObject == null || targetObject == null)
                return;

            casterID = casterObject.GameObjectId;
            targetID = targetObject.GameObjectId;
            path = MemoryHelper
                .ReadString(new nint(vfxPathPtr), Encoding.ASCII, 256);
        
            PluginLog.Verbose(
                $"[EC.VfxManager] VFX Caught." +
                $"Path: `{path}`, " +
                $"caster: {casterObject.Name}({casterObject.GameObjectId}), " +
                $"target: {targetObject.Name}({targetObject.GameObjectId})");
        }
        catch
        {
            return;
        }

        if (BlacklistedVfx.Contains(path))
            return;

        lock (TrackedEffects)
        {
            if(!TrackedEffects.ContainsKey(targetID))
                TrackedEffects[targetID] = [];
        }

        var info = new VfxInfo
        {
            VfxID     = vfxPtr.ToInt64(),
            CasterID  = casterID,
            TargetID  = targetID,
            Path      = path,
            SpawnTick = spawnTick,
        };
        
        lock (TrackedEffects)
            TrackedEffects[targetID].Add(info);
    }

    private static void EmptyVfxList(ushort _) {
        lock (TrackedEffects)
            TrackedEffects.Clear();
    }

    private static void EmptyVfxList(nint objectAddress)
    {
        if(!Svc.ClientState.IsLoggedIn || !GenericHelpers.IsScreenReady() ||
           !Player.Available || Player.IsBusy)
            return;

        var actorObject = Svc.Objects
            .FirstOrDefault(x => x.Address == objectAddress);

        if (actorObject == null)
            return;

        var actorID     = actorObject.GameObjectId;

        lock (TrackedEffects)
            TrackedEffects.Remove(actorID);
    }

    private static void RemoveSpecificVfx(nint vfxAddress)
    {
        lock (TrackedEffects)
        {
            var keys = TrackedEffects.Keys.ToList();
            foreach(var actorId in keys)
            {
                if(!TrackedEffects.TryGetValue(actorId, out var list) ||
                   list.Count == 0)
                    continue;

                var newList = new List<VfxInfo>(list.Count);
                newList.AddRange(list
                    .Where(info => info.VfxID != vfxAddress.ToInt64()));

                if(newList.Count == 0)
                    TrackedEffects.Remove(actorId);
                else
                    TrackedEffects[actorId] = newList;
            }
        }
    }
    
    private static void EmptyVfxListPeriodically(IFramework _)
    {
        if(!Svc.Condition[ConditionFlag.InCombat])
        {
            lock (TrackedEffects)
            {
                TrackedEffects.Clear();
                return;
            }
        }

        if (EzThrottler.Throttle("VfxManager_PeriodicEmpty", 140))
        {
            lock (TrackedEffects)
            {
                var keys = TrackedEffects.Keys.ToList();
                foreach(var actorId in keys)
                {
                    if(!TrackedEffects.TryGetValue(actorId, out var list) ||
                       list.Count == 0)
                        continue;

                    var newList = new List<VfxInfo>(list.Count);
                    newList.AddRange(list
                        .Where(info => info.AgeDuration < VfxExpiryDuration));

                    if(newList.Count == 0)
                        TrackedEffects.Remove(actorId);
                    else
                        TrackedEffects[actorId] = newList;
                }
            }
        }
    }
}

public record struct VfxInfo
{
    public long VfxID;

    public ulong CasterID;

    public ulong TargetID;

    public string Path;

    public long SpawnTick;

    public long Age => Environment.TickCount64 - SpawnTick;

    public float AgeSeconds => Age / 1000f;

    public TimeSpan AgeDuration => TimeSpan.FromSeconds(AgeSeconds);
}