using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Player;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Numerics;
using Aetheryte = Lumina.Excel.Sheets.Aetheryte;
using GrandCompany = ECommons.ExcelServices.GrandCompany;
#nullable disable

namespace ECommons.GameHelpers.LegacyPlayer;

/// <summary>
/// In general, these properties and methods should be made in a way that does not throws <see cref="NullReferenceException"/>, where feasible.
/// </summary>

[Obsolete("Use GameHelpers.Player")]
public static unsafe class Player
{
    public static readonly Number MaxLevel = 100;
    public static IPlayerCharacter Object => Svc.Objects.LocalPlayer;
    public static bool Available => Object != null;
    public static bool AvailableThreadSafe => GameObjectManager.Instance()->Objects.IndexSorted[0].Value != null;
    public static bool Interactable => Available && Object.IsTargetable;
    public static bool IsBusy => GenericHelpers.IsOccupied() || Object.IsCasting || IsMoving || IsAnimationLocked || Svc.Condition[ConditionFlag.InCombat];
    public static ulong CID => Svc.PlayerState.ContentId;
    public static StatusList Status => Object?.StatusList;
    public static string? Name => Object?.Name.ToString();
    public static string? NameWithWorld => GetNameWithWorld(Object);
    public static string? GetNameWithWorld(this IPlayerCharacter pc) => pc == null ? null : (pc.Name.ToString() + "@" + pc.HomeWorld.ValueNullable?.Name.ToString());
    public static RowRef<Race> Race => Svc.PlayerState.Race;
    public static Sex Sex => Svc.PlayerState.Sex;

    /// <remarks>Is unaffected by Level Sync</remarks>
    public static int Level => Object?.Level ?? 0;
    public static bool IsLevelSynced => PlayerState.Instance()->IsLevelSynced;
    public static int SyncedLevel => ECommons.GameHelpers.Player.SyncedLevel;
    [Obsolete("Use Level instead")]
    public static int UnsyncedLevel => GetUnsyncedLevel(GetJob(Object));
    public static int GetUnsyncedLevel(Job job) => Svc.PlayerState.GetClassJobLevel(job.GetGameData().Value);

    public static bool IsInHomeWorld => Available && Object.CurrentWorld.RowId == Object.HomeWorld.RowId;
    public static bool IsInHomeDC => Available && Object.CurrentWorld.Value.DataCenter.RowId == Object.HomeWorld.Value.DataCenter.RowId;
    public static string HomeWorld => Object.HomeWorld.Value.Name.ToString();
    public static string CurrentWorld => Object.CurrentWorld.Value.Name.ToString();
    public static string HomeDataCenter => Object.HomeWorld.Value.DataCenter.Value.Name.ToString();
    public static string CurrentDataCenter => Object.CurrentWorld.Value.DataCenter.Value.Name.ToString();

    public static Character* Character => (Character*)Object.Address;
    public static BattleChara* BattleChara => (BattleChara*)Object.Address;
    public static GameObject* GameObject => (GameObject*)Object.Address;

    public static uint Territory => Svc.ClientState.TerritoryType;
    public static TerritoryIntendedUseEnum TerritoryIntendedUse => (TerritoryIntendedUseEnum)(Svc.Data.GetExcelSheet<TerritoryType>().GetRowOrDefault(Territory)?.TerritoryIntendedUse.ValueNullable?.RowId ?? default);
    public static uint HomeAetheryteTerritory => Svc.Data.GetExcelSheet<Aetheryte>().GetRowOrDefault(PlayerState.Instance()->HomeAetheryteId).Value.Territory.RowId;
    public static bool IsInDuty => GameMain.Instance()->CurrentContentFinderConditionId != 0;
    public static bool IsOnIsland => MJIManager.Instance()->IsPlayerInSanctuary;
    public static bool IsInPvP => GameMain.IsInPvPInstance();

    public static RowRef<ClassJob> ClassJob => Object?.ClassJob ?? default;
    public static Job Job => Object?.GetJob() ?? 0;
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;
    public static Job GetJob(this IPlayerCharacter pc) => (Job)(pc?.ClassJob.RowId ?? 0);

    public static uint HomeWorldId => Object.HomeWorld.RowId;
    public static uint CurrentWorldId => Object.CurrentWorld.RowId;
    public static uint JobId => Object.ClassJob.RowId;
    public static uint OnlineStatus => Player.Object?.OnlineStatus.RowId ?? 0;

    public static Vector3 Position => Available ? Object.Position : Vector3.Zero;
    public static float Rotation => Available ? Object.Rotation : 0;
    public static bool IsMoving => Available && (AgentMap.Instance()->IsPlayerMoving || IsJumping);
    public static bool IsJumping => Available && (Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61] || Character->IsJumping());
    public static bool Mounted => Svc.Condition[ConditionFlag.Mounted];
    public static bool Mounting => Svc.Condition[ConditionFlag.MountOrOrnamentTransition];
    public static bool CanMount => Svc.Data.GetExcelSheet<TerritoryType>().GetRow(Territory).Mount && PlayerState.Instance()->NumOwnedMounts > 0;
    public static bool CanFly => Control.CanFly;

    public static float AnimationLock => ActionManager.Instance()->AnimationLock;
    public static bool IsAnimationLocked => AnimationLock > 0;
    public static bool IsCasting => Available && Object.IsCasting();
    public static bool IsDead => Svc.Condition[ConditionFlag.Unconscious];
    public static bool Revivable => IsDead && AgentRevive.Instance()->ReviveState != 0;

    public static float DistanceTo(Vector3 other) => Vector3.Distance(Position, other);
    public static float DistanceTo(Vector2 other) => Vector2.Distance(Position.ToVector2(), other);
    public static float DistanceTo(IGameObject other) => Vector3.Distance(Position, other.Position);

    [Obsolete("Use IsJumping")]
    public static unsafe bool Dismounting => **(byte**)(Svc.ClientState.LocalPlayer.Address + 1400) == 1;
    [Obsolete("Use IsJumping")]
    public static bool Jumping => Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61];
}
