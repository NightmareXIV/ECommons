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

namespace ECommons.GameHelpers;

/// <summary>
/// In general, these properties and methods should be made in a way that does not throws <see cref="NullReferenceException"/>, where feasible.
/// </summary>
public static unsafe class Player
{
    public static IPlayerCharacter? Object => Svc.Objects.LocalPlayer;
    public static Character* Character => (Character*)(Object?.Address ?? nint.Zero);
    public static BattleChara* BattleChara => (BattleChara*)(Object?.Address ?? nint.Zero);
    public static GameObject* GameObject => (GameObject*)(Object?.Address ?? nint.Zero);

    public static bool Available => GameObjectManager.Instance()->Objects.IndexSorted[0].Value != null;
    public static bool Interactable => Object?.IsTargetable ?? false;
    /// <summary>Checks if the player is occupied, casting, moving, animation locked, or in combat. Anything that would prevent most automation.</summary>
    public static bool IsBusy
        => GenericHelpers.IsOccupied()
        || (Object?.IsCasting ?? false)
        || IsMoving
        || IsAnimationLocked
        || Svc.Condition[ConditionFlag.InCombat]
        || GameMain.Instance()->TerritoryLoadState != 2;

    public static string? Name => Object.Name.ToString();
    public static string NameWithWorld => GetNameWithWorld(Object) ?? string.Empty;
    public static string GetNameWithWorld(this IPlayerCharacter? pc) => pc == null ? string.Empty : (pc.Name.ToString() + "@" + pc.HomeWorld.ValueNullable?.Name.ToString());
    public static ulong CID => Svc.PlayerState.ContentId;
    public static StatusList Status => Object?.StatusList ?? default!;
    public static Sex Sex => Svc.PlayerState.Sex;

    /// <remarks>Unsynced level</remarks>
    public static int Level => Object?.Level ?? 0;
    public static Number MaxLevel => PlayerState.Instance()->MaxLevel;
    public static bool IsLevelSynced => PlayerState.Instance()->IsLevelSynced;
    public static int SyncedLevel => PlayerState.Instance()->SyncedLevel;
    /// <remarks>Unsynced level</remarks>
    public static int GetLevel(Job job) => Svc.PlayerState.GetClassJobLevel(job.GetGameData().Value);

    #region Excel
    public static RowRef<Race> Race => Svc.PlayerState.Race;
    public static RowRef<Tribe> Tribe => Lumina.Excel.Sheets.Tribe.GetRef(PlayerState.Instance()->Tribe);
    public static RowRef<World> HomeWorld => Object?.HomeWorld ?? default;
    public static RowRef<World> CurrentWorld => Object?.CurrentWorld ?? default;
    public static RowRef<WorldDCGroupType> HomeDateCenter => HomeWorld.Value.DataCenter;
    public static RowRef<WorldDCGroupType> CurrentDataCenter => CurrentWorld.Value.DataCenter;
    public static RowRef<TerritoryType> Territory => TerritoryType.GetRef(Svc.ClientState.TerritoryType);
    public static RowRef<TerritoryIntendedUse> TerritoryIntendedUse => Territory.Value.TerritoryIntendedUse;
    public static RowRef<TerritoryType> HomeAetheryteTerritory => Aetheryte.GetRef(PlayerState.Instance()->HomeAetheryteId).Value.Territory;
    public static RowRef<ClassJob> ClassJob => Object?.ClassJob ?? default;
    public static RowRef<OnlineStatus> OnlineStatus => Object?.OnlineStatus ?? default;
    public static RowRef<ContentFinderCondition> ContentFinderCondition => Lumina.Excel.Sheets.ContentFinderCondition.GetRef(GameMain.Instance()->CurrentContentFinderConditionId);

    public static string HomeWorldName => HomeWorld.Value.Name.ToString();
    public static string CurrentWorldName => CurrentWorld.Value.Name.ToString();
    public static string HomeDataCenterName => HomeWorld.Value.DataCenter.Value.Name.ToString();
    public static string CurrentDataCenterName => CurrentWorld.Value.DataCenter.Value.Name.ToString();
    #endregion

    public static FFXIVClientStructs.FFXIV.Client.Enums.TerritoryIntendedUse CsTerritoryIntendedUseEnum => (FFXIVClientStructs.FFXIV.Client.Enums.TerritoryIntendedUse)TerritoryIntendedUse.RowId;
    public static TerritoryIntendedUseEnum TerritoryIntendedUseEnum => (TerritoryIntendedUseEnum)TerritoryIntendedUse.RowId;

    public static bool IsInDuty => GameMain.Instance()->CurrentContentFinderConditionId != 0;
    public static bool IsOnIsland => MJIManager.Instance()->IsPlayerInSanctuary;
    public static bool IsInPvP => GameMain.IsInPvPInstance();
    public static bool IsPenalised => FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent.Instance()->GetPenaltyRemainingInMinutes(0) > 0;
    public static bool IsInHomeWorld => Available && CurrentWorld.RowId == HomeWorld.RowId;
    public static bool IsInHomeDC => Available && CurrentWorld.Value.DataCenter.RowId == HomeWorld.Value.DataCenter.RowId;

    public static Job Job => (Job)ClassJob.RowId;
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;

    public static Vector3 Position => Object?.Position ?? default;
    public static float Rotation => Object?.Rotation ?? default;
    public static bool IsMoving => Available && (AgentMap.Instance()->IsPlayerMoving || IsJumping);
    public static bool IsJumping => Available && (Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61] || Character->IsJumping());
    public static bool Mounted => Svc.Condition[ConditionFlag.Mounted];
    public static bool Mounting => Svc.Condition[ConditionFlag.MountOrOrnamentTransition];
    /// <summary>Checks if the territory supports mounting, and if the player owns mounts</summary>
    public static bool CanMount => Territory.Value.Mount && PlayerState.Instance()->NumOwnedMounts > 0;
    /// <summary>Checks if the player can fly at the given moment. Requires the player to be mounted and in a territory that supports flying.</summary>
    public static bool CanFly => Control.CanFly;

    public static float AnimationLock => ActionManager.Instance()->AnimationLock;
    public static bool IsAnimationLocked => AnimationLock > 0;
    public static bool IsCasting => Object?.IsCasting() ?? false;
    public static bool IsDead => Svc.Condition[ConditionFlag.Unconscious];
    public static bool Revivable => IsDead && AgentRevive.Instance()->ReviveState != 0;

    public static float DistanceTo(Vector3 other) => Vector3.Distance(Position, other);
    public static float DistanceTo(Vector2 other) => Vector2.Distance(Position.ToVector2(), other);
    public static float DistanceTo(IGameObject other) => Vector3.Distance(Position, other.Position);
}
