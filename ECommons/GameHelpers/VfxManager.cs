#region

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
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

public static class VfxManager
{
    public static readonly List<VfxInfo> TrackedEffects = [];

    public static readonly TimeSpan VfxExpiryDuration = TimeSpan.FromSeconds(30);

    public static bool Logging = false;

    /// For filtering logging to only specific vfx paths or specific object IDs.
    /// Can be the full path or a substring.
    public static string? LoggingFilter = null;

    public static List<string> WhitelistedVfxPathSearches { get; set; } = [];

    public static bool TryGetVfxFor
    (ulong objectId, out List<VfxInfo> vfxList,
        bool searchCasterAsWell = false) =>
        TryGetVfx(objectId, null, out vfxList, searchCasterAsWell);

    public static bool TryGetVfxLike
        (string pathSearch, out List<VfxInfo> vfxList) =>
        TryGetVfx(null, pathSearch, out vfxList);

    private static bool TryGetVfx
    (ulong? objectId, string? pathSearch, out List<VfxInfo> vfxList,
        bool searchCasterAsWell = false)
    {
        vfxList = [];

        var hasObjectFilter = objectId.HasValue && objectId.Value != 0;
        var hasPathFilter   = !string.IsNullOrWhiteSpace(pathSearch);

        if(!hasObjectFilter && !hasPathFilter)
            return false;

        lock(TrackedEffects)
        {
            foreach(var info in TrackedEffects)
            {
                if(hasObjectFilter &&
                   info.TargetID != objectId!.Value &&
                   (!searchCasterAsWell || info.CasterID != objectId.Value))
                    continue;

                if(hasPathFilter &&
                   !info.Path.Contains(pathSearch!, Lower))
                    continue;

                vfxList.Add(info);
            }
        }

        return vfxList.Count > 0;
    }

    private static unsafe void TrackOnVfxCreate
    (nint vfxPtr, char* vfxPathPtr, nint casterAddress, nint targetAddress,
        float a4, char a5, ushort a6, char a7)
    {
        var    vfxID     = vfxPtr.ToInt64();
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
        catch(Exception ex)
        {
            if(Logging)
            {
                var log = ex.ToStringFull();
                if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                    PluginLog.Debug(
                        $"[EC.VfxManager] VFX #{vfxID} FAILED to Catch. {log}");
            }

            return;
        }

        // Skip VFX that do not match the whitelist (if set up)
        if(WhitelistedVfxPathSearches.Count > 0)
        {
            if(!WhitelistedVfxPathSearches.Any(x =>
                   path.Contains(x, Lower)))
            {
                if(Logging)
                {
                    var log = $"Path: `{path}`, " +
                              $"Caster: {casterID}, " +
                              $"Target: {targetID}";
                    if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                        PluginLog.Debug(
                            $"[EC.VfxManager] VFX #{vfxID} SKIPPED Catching" +
                            $"(not whitelisted). {log}");
                }

                return;
            }
        }

        var info = new VfxInfo
        {
            VfxID     = vfxID,
            CasterID  = casterID,
            TargetID  = targetID,
            Path      = path,
            SpawnTick = spawnTick,
        };

        lock(TrackedEffects)
            TrackedEffects.Add(info);

        if(Logging)
        {
            var log = $"Path: `{path}`, " +
                      $"Caster: {casterID}, " +
                      $"Target: {targetID}";
            if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                PluginLog.Verbose(
                    $"[EC.VfxManager] VFX #{vfxID} Caught. {log}");
        }
    }

    #region Search `out` Filter Extensions

    extension(List<VfxInfo> vfxList)
    {
        public List<VfxInfo> FilterToTarget(ulong targetID) =>
            vfxList.Where(x => x.TargetID == targetID).ToList();

        public List<VfxInfo> FilterToCaster(ulong casterID) =>
            vfxList.Where(x => x.CasterID == casterID).ToList();

        public List<VfxInfo> FilterToNoTarget() =>
            vfxList.Where(x => x.TargetID == ulong.MaxValue).ToList();

        public List<VfxInfo> FilterToNoCaster() =>
            vfxList.Where(x => x.CasterID == ulong.MaxValue).ToList();

        public List<VfxInfo> FilterToPath(string pathSearch) =>
            vfxList.Where(x => x.Path.Contains(pathSearch, Lower)).ToList();

        public List<VfxInfo> FilterToExactPath(string path) =>
            vfxList.Where(x => x.Path.Equals(path, Lower)).ToList();

        public List<VfxInfo> FilterYoungerThan(TimeSpan duration) =>
            vfxList.Where(x => x.AgeDuration < duration).ToList();

        public List<VfxInfo> FilterToTargetRole(CombatRole role) =>
            vfxList.Where(x =>
            {
                var obj = Svc.Objects
                    .FirstOrDefault(o => o is ICharacter c &&
                                         c.GameObjectId == x.TargetID);
                return obj is ICharacter chara && chara.GetRole() == role;
            }).ToList();
    }

    #endregion

    #region Boilerplate

    private const StringComparison Lower =
        StringComparison.OrdinalIgnoreCase;

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
        lock(TrackedEffects)
            TrackedEffects.Clear();
        ActorVfx.ActorVfxCreateEvent              -= TrackOnVfxCreate;
        ActorVfx.ActorVfxDtorEvent                -= RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent -= EmptyVfxList;
        Svc.ClientState.TerritoryChanged          -= EmptyVfxList;
        Svc.Framework.Update                      -= EmptyVfxListPeriodically;
    }

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
        var vfxID    = vfxAddress.ToInt64();
        var realVfx  = (VfxStruct*)vfxAddress;
        var casterID = realVfx->CasterID;
        var targetID = realVfx->TargetID;

        int removed;
        lock(TrackedEffects)
            removed = TrackedEffects.RemoveAll(info =>
                info.VfxID == vfxID &&
                info.CasterID == casterID &&
                info.TargetID == targetID);

        if(Logging)
        {
            var log = $"Removed {removed} Tracked VFX. " +
                      $"Caster:{casterID}, " +
                      $"Target:{targetID}";
            if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                PluginLog.Verbose(
                    $"[EC.VfxManager] VFX #{vfxID} Destroyed. {log}");
        }
    }

    private static bool _lastTickInCombatFlag;

    private static void EmptyVfxListPeriodically(IFramework _)
    {
        // Clear tracked VFXs only when transitioning out of combat
        var inCombat = Svc.Condition[ConditionFlag.InCombat];
        if(!inCombat && _lastTickInCombatFlag)
        {
            lock(TrackedEffects)
                TrackedEffects.Clear();
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