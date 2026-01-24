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
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

#endregion

namespace ECommons.GameHelpers;

/// <summary>
///     Tracks all VFXs created in the game, culling them when the game removes
///     them, or after it should have, and keeps them ready for you to query
///     with the <c>TryGet</c> Methods or directly from
///     <see cref="TrackedEffects" />.
/// </summary>
public static class VfxManager
{
    /// <summary>
    ///     List of all currently tracked VFXs.
    /// </summary>
    public static readonly List<VfxInfo> TrackedEffects = [];

    /// <summary>
    ///     How long to keep VFXs tracked after their creation before
    ///     automatically removing them.<br />
    ///     (Should not generally be needed, as they are un-tracked on destruction)
    /// </summary>
    public static readonly TimeSpan VfxExpiryDuration = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Whether VFXs should be un-tracked on their destruction event.
    /// </summary>
    public static readonly bool EnableDtorCulling = true;

    /// <summary>
    ///     Whether Static VFX should be tracked from creation time, or only
    ///     when they are Ran.<br />
    ///     Similar to using the filter <see cref="FilterToReady" />.
    /// </summary>
    public static bool EnableStaticVfxCreationTracking { get; set; } = false;

    /// <summary>
    ///     Whether VfxManager should log anything at all.
    /// </summary>
    public static bool Logging { get; set; } = false;

    /// <summary>
    ///     If set, will restrict logging to logs whose bodies contain this string.
    ///     <br />
    ///     Useful for filtering to your player ID or specific VFX paths.
    /// </summary>
    public static string? LoggingFilter { get; set; }

    /// <summary>
    ///     If populated, will only start tracking of VFXs whose paths
    ///     contain at least one of the strings in this list.
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public static List<string> WhitelistedVfxPathSearches { get; set; } = [];

    /// <summary>
    ///     Searches <see cref="TrackedEffects" /> for VFXs matching the given
    ///     Object ID.
    /// </summary>
    /// <param name="objectID">The Object to search for.</param>
    /// <param name="vfxList">
    ///     List of discovered <see cref="VfxInfo">VFXs</see> (empty if
    ///     <see langword="false" /> is returned).
    ///     <br />
    ///     Can then be filtered further with the extension methods provided in
    ///     VfxManager, <see cref="FilterToTarget" />, etc.
    /// </param>
    /// <param name="searchCasterAsWell">
    ///     Whether the VFX's CasterID should be searched in addition to
    ///     the TargetID.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if any matching VFXs were found.
    /// </returns>
    public static bool TryGetVfxFor
    (ulong objectID, out List<VfxInfo> vfxList,
        bool searchCasterAsWell = false) =>
        TryGetVfx(objectID, null, out vfxList, searchCasterAsWell);

    /// <summary>
    ///     Searches <see cref="TrackedEffects" /> for VFXs matching the given
    ///     Path.
    /// </summary>
    /// <param name="pathSearch">
    ///     The Path to search for.<br />
    ///     Checks if the VFX's path contains this string.
    /// </param>
    /// <param name="vfxList">
    ///     List of discovered <see cref="VfxInfo">VFXs</see> (empty if
    ///     <see langword="false" /> is returned).
    ///     <br />
    ///     Can then be filtered further with the extension methods provided in
    ///     VfxManager, <see cref="FilterToTarget" />, etc.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if any matching VFXs were found.
    /// </returns>
    public static bool TryGetVfxLike
        (string pathSearch, out List<VfxInfo> vfxList) =>
        TryGetVfx(null, pathSearch, out vfxList);

    /// <summary>
    ///     Searches <see cref="TrackedEffects" /> for VFXs matching any given
    ///     Object ID and any given path.
    /// </summary>
    /// <param name="objectID">The Object to search for.</param>
    /// <param name="pathSearch">
    ///     The Path to search for.<br />
    ///     Checks if the VFX's path contains this string.
    /// </param>
    /// <param name="vfxList">
    ///     List of discovered <see cref="VfxInfo">VFXs</see> (empty if
    ///     <see langword="false" /> is returned).
    ///     <br />
    ///     Can then be filtered further with the extension methods provided in
    ///     VfxManager, <see cref="FilterToTarget" />, etc.
    /// </param>
    /// <param name="searchCasterAsWell">
    ///     Whether the VFX's CasterID should be searched in addition to
    ///     the TargetID.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if any matching VFXs were found.
    /// </returns>
    private static bool TryGetVfx
    (ulong? objectID, string? pathSearch, out List<VfxInfo> vfxList,
        bool searchCasterAsWell = false)
    {
        vfxList = [];

        var hasObjectFilter = objectID.HasValue && objectID.Value != 0;
        var hasPathFilter   = !string.IsNullOrWhiteSpace(pathSearch);

        if(!hasObjectFilter && !hasPathFilter)
            return false;

        lock(TrackedEffects)
        {
            foreach(var info in TrackedEffects)
            {
                if(hasObjectFilter &&
                   info.TargetID != objectID!.Value &&
                   (!searchCasterAsWell || info.CasterID != objectID.Value))
                    continue;

                if(hasPathFilter &&
                   !info.Path.Contains(pathSearch!, Lower))
                    continue;

                vfxList.Add(info);
            }
        }

        return vfxList.Count > 0;
    }

    /// <summary>
    ///     Centralized VFX tracking logic used by actor and static hooks.
    /// </summary>
    /// <param name="vfx">Pointer to the VFX struct being tracked.</param>
    /// <param name="casterID">Resolved caster game object ID.</param>
    /// <param name="targetID">Resolved target game object ID.</param>
    /// <param name="path">
    ///     Resolved VFX asset path
    ///     (can be empty for
    ///     <see cref="TrackOnStaticVfxRun">Static VFX Run Event</see>s).
    /// </param>
    /// <param name="isStatic">Whether this VFX came from the static hook.</param>
    /// <param name="hasRun">
    ///     Whether the VFX has been run
    ///     (for Static VFX events).<br />
    ///     More specifically, whether it came from a
    ///     <see cref="TrackOnStaticVfxRun">Static VFX Run Event</see>.
    /// </param>
    /// <seealso cref="TrackOnActorVfxCreate" />
    /// <seealso cref="TrackOnStaticVfxCreate" />
    /// <seealso cref="TrackOnStaticVfxRun" />
    private static unsafe void TrackVfx
    (VfxStruct* vfx, ulong casterID, ulong targetID,
        string path, bool isStatic, bool hasRun)
    {
        if(vfx == null)
            return;

        var vfxID     = ((nint)vfx).ToInt64();
        var spawnTick = Environment.TickCount64;

        // Skip VFX that do not match the whitelist (if set up)
        if(WhitelistedVfxPathSearches.Count > 0 &&
           !WhitelistedVfxPathSearches.Any(x => path.Contains(x, Lower)))
        {
            if(Logging)
            {
                var log = $"Path: `{path}`, Caster: {casterID}, Target: {targetID}";
                if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                    PluginLog.Debug(
                        $"[EC.VfxManager] VFX #{vfxID:X8} SKIPPED Catching " +
                        $"(not whitelisted). {log}");
            }

            return;
        }

        VfxInfo.PlacementData? placement = null;
        try
        {
            // Capture placement data when available
            placement = new VfxInfo.PlacementData
            {
                Position = vfx->Position,
                Rotation = vfx->Rotation,
                Scale    = vfx->Scale,
            };
        }
        catch
        {
            // Ignore placement extraction failures
        }

        VfxInfo info;
        lock(TrackedEffects)
        {
            var index = TrackedEffects.FindIndex(x => x.VfxID == vfxID);
            if(index >= 0)
            {
                info           =  TrackedEffects[index];
                info.SpawnTick =  spawnTick;
                info.HasRun    |= hasRun;

                if(info.CasterID == ulong.MaxValue && casterID != ulong.MaxValue)
                    info.CasterID = casterID;
                if(info.TargetID == ulong.MaxValue && targetID != ulong.MaxValue)
                    info.TargetID = targetID;
                if(!string.IsNullOrEmpty(path))
                    info.Path = path;
                if(info.Placement is null && placement is not null)
                    info.Placement = placement;

                TrackedEffects[index] = info;
            }
            else
            {
                info = new VfxInfo
                {
                    VfxID     = vfxID,
                    CasterID  = casterID,
                    TargetID  = targetID,
                    Path      = path,
                    SpawnTick = spawnTick,
                    Placement = placement,
                    IsStatic  = isStatic,
                    HasRun    = hasRun,
                };

                TrackedEffects.Add(info);
            }
        }

        if(Logging)
        {
            var log =
                $"Path: `{info.Path}`, Caster: {info.CasterID}, " +
                $"Target: {info.TargetID}, Static: {info.IsStatic}, " +
                $"HasRun: {info.HasRun}";
            var verb = hasRun && EnableStaticVfxCreationTracking
                ? "Updated"
                : "Caught";
            if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                PluginLog.Verbose(
                    $"[EC.VfxManager] VFX #{info.VfxID:X8} {verb}. {log}");
        }
    }

    #region Search `out` Filter Extensions

    extension(List<VfxInfo> vfxList)
    {
        /// Returns only the VFXs that are either non-static, or that have run.
        public List<VfxInfo> FilterToReady() =>
            vfxList.Where(x => !x.IsStatic || x.HasRun).ToList();

        /// Returns only the VFXs with defined targets.
        public List<VfxInfo> FilterToTargeted() =>
            vfxList.Where(x => x.TargetID != ulong.MaxValue).ToList();

        /// Returns only the VFXs targeting the given Object ID.
        public List<VfxInfo> FilterToTarget(ulong targetID) =>
            vfxList.Where(x => x.TargetID == targetID).ToList();

        /// Returns only the VFXs cast by the given Object ID.
        /// <remarks>
        ///     The casting Actor of a VFX is not necessarily the enemy initiating
        ///     the VFX, e.g. tank busters often have the tank as the caster.
        /// </remarks>
        public List<VfxInfo> FilterToCaster(ulong casterID) =>
            vfxList.Where(x => x.CasterID == casterID).ToList();

        /// Returns only the VFXs with no defined target.
        public List<VfxInfo> FilterToNoTarget() =>
            vfxList.Where(x => x.TargetID == ulong.MaxValue).ToList();

        /// Returns only the VFXs with no defined caster.
        public List<VfxInfo> FilterToNoCaster() =>
            vfxList.Where(x => x.CasterID == ulong.MaxValue).ToList();

        /// Returns only the VFXs whose paths contain the given string.
        public List<VfxInfo> FilterToPath(string pathSearch) =>
            vfxList.Where(x => x.Path.Contains(pathSearch, Lower)).ToList();

        /// Returns only the VFXs whose paths exactly match the given string.
        public List<VfxInfo> FilterToExactPath(string path) =>
            vfxList.Where(x => x.Path.Equals(path, Lower)).ToList();

        /// Returns only the VFXs younger than the given duration.
        public List<VfxInfo> FilterYoungerThan(TimeSpan duration) =>
            vfxList.Where(x => x.AgeDuration < duration).ToList();

        /// Returns only the VFXs targeting Characters with the given Role.
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

    #region Unique VFX Tracking Creation Methods (all go to `TrackVfx`)

    /// <summary>
    ///     Our Delegate subscribed to
    ///     <see cref="ActorVfx.ActorVfxCreateEvent" /> in
    ///     <see cref="Init" /> to track created VFXs.
    /// </summary>
    /// <seealso cref="ActorVfx.ActorVfxCreateCallbackDelegate" />
    private static unsafe void TrackOnActorVfxCreate
    (nint vfxPtr, nint vfxPathPtr, nint casterAddress, nint targetAddress,
        float a4, byte a5, ushort a6, byte a7)
    {
        var    vfx   = (VfxStruct*)vfxPtr;
        var    vfxID = vfxPtr.ToInt64();
        ulong  casterID;
        ulong  targetID;
        string path;

        try
        {
            // Resolve caster/target IDs from object addresses.
            casterID =
                Svc.Objects.FirstOrDefault(x => x.Address == casterAddress)
                    ?.GameObjectId ?? ulong.MaxValue;
            targetID =
                Svc.Objects.FirstOrDefault(x => x.Address == targetAddress)
                    ?.GameObjectId ?? ulong.MaxValue;
            path = MemoryHelper.ReadString(new nint(vfxPathPtr), Encoding.ASCII,
                256);
        }
        catch(Exception ex)
        {
            if(Logging)
            {
                var log = ex.ToStringFull();
                if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                    PluginLog.Debug(
                        $"[EC.VfxManager] VFX #{vfxID:X8} FAILED to Catch. {log}");
            }

            return;
        }

        TrackVfx(vfx, casterID, targetID, path, isStatic: false, hasRun: false);
    }

    /// <summary>
    ///     Tracks Static VFX creation events.
    /// </summary>
    private static unsafe void TrackOnStaticVfxCreate(nint vfxPtr, string path,
        string systemSource)
    {
        var vfx      = (VfxStruct*)vfxPtr;
        var vfxID    = vfxPtr.ToInt64();

        // More than likely, these are ulong.MaxValue until Run event
        var casterID = vfx->StaticCasterID;
        var targetID = vfx->StaticTargetID;

        // Cache path for use during Run events (even if we skip creation tracking)
        lock (StaticVfxPathCache)
            StaticVfxPathCache[vfxID] = path;
        // Bail if we don't want to start tracking yet
        if(!EnableStaticVfxCreationTracking)
            return;

        TrackVfx(vfx, casterID, targetID, path, isStatic: true, hasRun: false);
    }

    /// <summary>
    ///     Tracks Static VFX run events, ensuring HasRun is marked.
    /// </summary>
    private static unsafe void TrackOnStaticVfxRun(nint vfxPtr, float a1, uint a2)
    {
        var vfx      = (VfxStruct*)vfxPtr;
        var vfxID    = vfxPtr.ToInt64();
        var casterID = vfx->StaticCasterID;
        var targetID = vfx->StaticTargetID;
        var path     = string.Empty;

        // Reuse cached path from creation if available
        lock (StaticVfxPathCache)
            if (StaticVfxPathCache.TryGetValue(vfxID, out var cachedPath))
                path = cachedPath;

        TrackVfx(vfx, casterID, targetID, path, isStatic: true, hasRun: true);
    }

    #endregion

    #region Boilerplate

    private const StringComparison Lower =
        StringComparison.OrdinalIgnoreCase;

    /// <summary>
    ///     Caches static VFX paths captured during creation so run events can
    ///     reuse them even when creation tracking is disabled.
    /// </summary>
    private static readonly Dictionary<long, string> StaticVfxPathCache = [];

    internal static void Init()
    {
        ActorVfx.ActorVfxCreateEvent              += TrackOnActorVfxCreate;
        ActorVfx.ActorVfxDtorEvent                += RemoveSpecificVfx;
        StaticVfx.StaticVfxCreateEvent            += TrackOnStaticVfxCreate;
        StaticVfx.StaticVfxRunEvent               += TrackOnStaticVfxRun;
        StaticVfx.StaticVfxDtorEvent              += RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent += EmptyVfxList;
        Svc.ClientState.TerritoryChanged          += EmptyVfxList;
        Svc.Framework.Update                      += EmptyVfxListPeriodically;
    }

    internal static void Dispose()
    {
        lock(TrackedEffects)
            TrackedEffects.Clear();
        lock (StaticVfxPathCache)
            StaticVfxPathCache.Clear();
        ActorVfx.ActorVfxCreateEvent              -= TrackOnActorVfxCreate;
        ActorVfx.ActorVfxDtorEvent                -= RemoveSpecificVfx;
        StaticVfx.StaticVfxCreateEvent            -= TrackOnStaticVfxCreate;
        StaticVfx.StaticVfxRunEvent               -= TrackOnStaticVfxRun;
        StaticVfx.StaticVfxDtorEvent              -= RemoveSpecificVfx;
        GameObjectCtor.GameObjectConstructorEvent -= EmptyVfxList;
        Svc.ClientState.TerritoryChanged          -= EmptyVfxList;
        Svc.Framework.Update                      -= EmptyVfxListPeriodically;
    }

    #endregion

    #region Culling of VFXs

    /// Completely empties the VFX list (used on territory change).
    private static void EmptyVfxList(ushort _)
    {
        lock(TrackedEffects)
            TrackedEffects.Clear();
    }

    /// Empties VFXs related to the given GameObject (used on GameObject creation).
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

    /// Removes a specific VFX from tracking (used on VFX destruction).
    /// <remarks>
    ///     Removes all VFXs matching the given VFX pointer's ID
    ///     and Caster/Target IDs.<br />
    ///     (will also match VFXs with the same ID, but have now-undefined
    ///     target/casters - this can happen because it was undefined to begin
    ///     with, OR if the game object was destroyed before the VFX)
    /// </remarks>
    private static unsafe void RemoveSpecificVfx(nint vfxAddress)
    {
        if(!EnableDtorCulling)
            return;

        long  vfxID;
        ulong casterID;
        ulong targetID;
        try
        {
            vfxID = vfxAddress.ToInt64();
            var realVfx = (VfxStruct*)vfxAddress;
            casterID = realVfx->ActorCasterID != ulong.MaxValue
                ? realVfx->ActorCasterID
                : realVfx->StaticCasterID;
            targetID = realVfx->ActorTargetID != ulong.MaxValue
                ? realVfx->ActorTargetID
                : realVfx->StaticTargetID;
        }
        catch
        {
            return;
        }

        int removed;
        lock(TrackedEffects)
            removed = TrackedEffects.RemoveAll(info => info.VfxID == vfxID);

        // Drop any cached static path for this VFX
        lock(StaticVfxPathCache)
            StaticVfxPathCache.Remove(vfxID);

        if(Logging)
        {
            var log = $"Removed {removed} Tracked VFX. " +
                      $"Caster: {casterID}, Target: {targetID}";
            if(LoggingFilter is null || log.Contains(LoggingFilter, Lower))
                PluginLog.Verbose(
                    $"[EC.VfxManager] VFX #{vfxID:X8} Destroyed. {log}");
        }
    }

    /// Used to track whether the player was previously in combat.
    private static bool _lastTickInCombatFlag;

    /// Periodically empties old VFXs, and empties all VFXs when
    /// transitioning out of combat.
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

/// <summary>
///     The stored data for a tracked VFX.
/// </summary>
/// <seealso cref="VfxManager.TrackedEffects" />
public record struct VfxInfo
{
    /// Identifier of the actor that spawned the VFX.
    public ulong CasterID;

    /// Whether a Static VFX was tracked before it ran.
    public bool HasRun;

    /// Whether the VFX is a Static VFX.
    public bool IsStatic;

    /// Source path of the spawned VFX asset.
    /// <remarks>
    ///     For Static VFXs, this field is less certain, and may even be empty,
    ///     when the VFX was created long before it was run (which is mostly
    ///     not the case when looking for VFX for mechanics, but still possible).
    /// </remarks>
    public string Path;

    public PlacementData? Placement;

    /// <summary>Tick count at which the VFX was spawned.</summary>
    /// <remarks>
    ///     If <see cref="IsStatic"/>, then will be updated once the
    ///     VFX is actually run.
    /// </remarks>
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

    public class PlacementData
    {
        public Vector3 Position;
        public Quat    Rotation;
        public Vector3 Scale;
    }
}

/// <summary>
///     The data behind a <c>nint vfxPtr</c>.
/// </summary>
/// <example>
///     <c>var vfx = (VfxStruct*)vfxPtr;</c>
/// </example>
/// <seealso href="https://github.com/0ceal0t/Dalamud-VFXEditor/blob/c320f08c981f3bf7353157c3d2faebcb00cba511/VFXEditor/Interop/Structs/Vfx/BaseVfx.cs#L29-L41">From VFXEditor</seealso>
[StructLayout(LayoutKind.Explicit)]
public struct VfxStruct
{
    /// Position of the VFX in world space.
    [FieldOffset(0x50)] public Vector3 Position;

    /// Rotations to apply to the VFX.
    [FieldOffset(0x60)] public Quat Rotation;

    /// Scale to apply to the VFX.
    [FieldOffset(0x70)] public Vector3 Scale;

    /// Identifier of the actor that spawned the VFX. (ActorVfx)
    [FieldOffset(0x128)] public ulong ActorCasterID;

    /// Identifier of the target the VFX is attached to. (ActorVfx)
    [FieldOffset(0x130)] public ulong ActorTargetID;

    /// Identifier of the actor that spawned the VFX. (StaticVfx)
    [FieldOffset(0x1B8)] public ulong StaticCasterID;

    /// Identifier of the target the VFX is attached to. (StaticVfx)
    [FieldOffset(0x1C0)] public ulong StaticTargetID;
}

/// <summary>
///     The quaternion struct used by the VFX system for rotations.
/// </summary>
/// <seealso href="https://github.com/0ceal0t/Dalamud-VFXEditor/blob/c320f08c981f3bf7353157c3d2faebcb00cba511/VFXEditor/Interop/Structs/Quat.cs">From VFXEditor</seealso>
[StructLayout(LayoutKind.Sequential)]
public struct Quat
{
    public float X;
    public float Z;
    public float Y;
    public float W;

    public static implicit operator Vector4(Quat pos) =>
        new(pos.X, pos.Y, pos.Z, pos.W);
}