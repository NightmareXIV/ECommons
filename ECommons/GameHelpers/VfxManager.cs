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
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace ECommons.GameHelpers;

public class VfxManager
{
    public static readonly List<VfxInfo> TrackedEffects = [];

    public static readonly TimeSpan VfxExpiryDuration = TimeSpan.FromSeconds(30);

    public static bool TryGetVfxFor(ulong objectId, out List<VfxInfo> vfxList) =>
        TryGetVfxFor(objectId, null, out vfxList);

    public static bool TryGetVfxFor(ulong objectId, string? pathSearch,
        out List<VfxInfo> vfxList)
    {
        vfxList = [];
        if(objectId == 0)
            return false;

        lock(TrackedEffects)
        {
            foreach(var info in TrackedEffects)
            {
                if(info.TargetID != objectId)
                    continue;

                if(!string.IsNullOrWhiteSpace(pathSearch) &&
                   !info.Path.Contains(pathSearch,
                       StringComparison.OrdinalIgnoreCase))
                    continue;

                vfxList.Add(info);
            }
        }

        return vfxList.Count > 0;
    }

    internal static unsafe void Init()
    {
        ActorVfx.ActorVfxCreateEvent              += TrackOnVfxCreate;
        ActorVfx.ActorVfxDtorEvent                += RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent += EmptyVfxList;
        Svc.ClientState.TerritoryChanged          += EmptyVfxList;
        Svc.Framework.Update                      += EmptyVfxListPeriodically;
    }

    private static unsafe void TrackOnVfxCreate
    (nint vfxPtr, char* vfxPathPtr, nint casterAddress, nint targetAddress,
        float a4, char a5, ushort a6, char a7)
    {
        var    spawnTick = Environment.TickCount64;
        ulong  casterID;
        ulong  targetID;
        string path;
        try
        {
            var casterObject = Svc.Objects
                .FirstOrDefault(x => x.Address == casterAddress);
            var targetObject = Svc.Objects
                .FirstOrDefault(x => x.Address == targetAddress);

            casterID = casterObject?.GameObjectId ?? ulong.MaxValue;
            targetID = targetObject?.GameObjectId ?? ulong.MaxValue;
            path = MemoryHelper
                .ReadString(new nint(vfxPathPtr), Encoding.ASCII, 256);
        }
        catch
        {
            return;
        }

        if(BlacklistedVfx.Contains(path))
            return;

        var info = new VfxInfo
        {
            VfxID     = vfxPtr.ToInt64(),
            CasterID  = casterID,
            TargetID  = targetID,
            Path      = path,
            SpawnTick = spawnTick,
        };

        lock(TrackedEffects)
            TrackedEffects.Add(info);

        PluginLog.Verbose(
            $"[EC.VfxManager] VFX #{vfxPtr.ToInt64()} Caught. " +
            $"Path: `{path}`, " +
            $"caster: {casterID}, " +
            $"target: {targetID}");
    }

    #region Boilerplate

    internal static unsafe void Dispose()
    {
        lock(TrackedEffects)
            TrackedEffects.Clear();
        ActorVfx.ActorVfxCreateEvent              -= TrackOnVfxCreate;
        ActorVfx.ActorVfxDtorEvent                -= RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent -= EmptyVfxList;
        Svc.ClientState.TerritoryChanged          -= EmptyVfxList;
        Svc.Framework.Update                      -= EmptyVfxListPeriodically;
    }

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

    #endregion

    #region Culling of VFXs

    private static void EmptyVfxList(ushort _)
    {
        lock(TrackedEffects)
            TrackedEffects.Clear();
    }

    private static void EmptyVfxList(nint objectAddress)
    {
        if(!Svc.ClientState.IsLoggedIn || !GenericHelpers.IsScreenReady() ||
           !Player.Available || Player.IsBusy)
            return;

        var actorObject = Svc.Objects
            .FirstOrDefault(x => x.Address == objectAddress);

        if(actorObject == null)
            return;

        ulong actorID;
        try
        {
            actorID = actorObject.GameObjectId;
        }
        catch
        {
            return;
        }

        lock(TrackedEffects)
            TrackedEffects.RemoveAll(x => x.TargetID == actorID);
    }

    private static unsafe void RemoveSpecificVfx(nint vfxAddress)
    {
        ulong casterID;
        ulong targetID;

        try
        {
            var realVfx = (VfxStruct*)vfxAddress;
            casterID = realVfx->CasterID;
            targetID = realVfx->TargetID;
        }
        catch
        {
            return;
        }

        lock(TrackedEffects)
        {
            var removed = TrackedEffects.RemoveAll(info =>
                info.VfxID == vfxAddress.ToInt64() &&
                (info.CasterID == casterID ||
                 casterID is ulong.MaxValue) &&
                (info.TargetID == targetID ||
                 targetID is ulong.MaxValue));

            PluginLog.Verbose(
                $"[EC.VfxManager] VFX #{vfxAddress.ToInt64()} Destroyed. " +
                $"caster: {casterID}, " +
                $"target: {targetID} - " +
                $"Removed {removed} tracked entries.");
        }
    }

    private static bool _lastTickInCombatFlag;

    private static void EmptyVfxListPeriodically(IFramework _)
    {
        // Clear tracked VFXs when exiting combat
        var inCombat = Svc.Condition[ConditionFlag.InCombat];
        if(!inCombat && _lastTickInCombatFlag)
        {
            lock(TrackedEffects)
            {
                TrackedEffects.Clear();
            }
        }

        _lastTickInCombatFlag = inCombat;

        // Early exit
        if(!inCombat)
            return;

        // Cull old VFXs periodically
        if(EzThrottler.Throttle("VfxManager_PeriodicEmpty", 140))
        {
            lock(TrackedEffects)
            {
                TrackedEffects.RemoveAll(info =>
                    info.AgeDuration >= VfxExpiryDuration);
            }
        }
    }

    #endregion
}

public record struct VfxInfo
{
    /// Identifier of the actor that spawned the VFX.
    public ulong CasterID;

    /// Source path of the spawned VFX asset.
    public string Path;

    /// Tick count at which the VFX was spawned.
    public long SpawnTick;

    /// Identifier of the target the VFX is attached to.
    public ulong TargetID;

    /// Unique identifier of the VFX instance.
    public long VfxID;

    /// How many ticks the VFX has existed for.
    public long Age => Environment.TickCount64 - SpawnTick;

    /// How many seconds the VFX has existed for.
    public float AgeSeconds => Age / 1000f;

    /// The TimeSpan that the VFX has existed for.
    public TimeSpan AgeDuration => TimeSpan.FromSeconds(AgeSeconds);
}

// https://github.com/0ceal0t/Dalamud-VFXEditor/blob/c320f08c981f3bf7353157c3d2faebcb00cba511/VFXEditor/Interop/Structs/Vfx/BaseVfx.cs#L29-L41
[StructLayout(LayoutKind.Explicit)]
public struct VfxStruct
{
    /// Identifier of the actor that spawned the VFX.
    [FieldOffset(0x128)] public ulong CasterID;

    /// Identifier of the target the VFX is attached to.
    [FieldOffset(0x130)] public ulong TargetID;
}