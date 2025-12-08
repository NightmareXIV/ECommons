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
    public static readonly Number MaxLevel = 100;
    public static IPlayerCharacter? Object => Svc.Objects.LocalPlayer;
    public static bool Available => Svc.Objects.LocalPlayer != null;
    public static bool AvailableThreadSafe => GameObjectManager.Instance()->Objects.IndexSorted[0].Value != null;
    public static bool Interactable => Object?.IsTargetable ?? false;
    public static bool IsBusy => GenericHelpers.IsOccupied() || Object.IsCasting || IsMoving || IsAnimationLocked || Svc.Condition[ConditionFlag.InCombat];
    public static ulong CID => Svc.PlayerState.ContentId;
    public static StatusList Status => Svc.Objects.LocalPlayer?.StatusList;

    public static string Name => Svc.PlayerState.CharacterName;
    public static string NameWithWorld => GetNameWithWorld(Object);
    public static string GetNameWithWorld(this IPlayerCharacter pc) => pc == null ? null : (pc.Name.ToString() + "@" + pc.HomeWorld.ValueNullable?.Name.ToString());
    public static Sex Sex => Svc.PlayerState.Sex;

    /// <remarks>Adjusts to sync, same as <see cref="SyncedLevel"/></remarks>
    public static int Level => Svc.PlayerState.Level;
    public static bool IsLevelSynced => PlayerState.Instance()->IsLevelSynced;
    public static int SyncedLevel => PlayerState.Instance()->SyncedLevel;
    public static int UnsyncedLevel => GetUnsyncedLevel(GetJob(Object));
    public static int GetUnsyncedLevel(Job job) => Svc.PlayerState.GetClassJobLevel(job.GetGameData().Value);

    #region Excel
    public static RowRef<Race> Race => Svc.PlayerState.Race;
    public static RowRef<World> HomeWorld => Svc.PlayerState.HomeWorld;
    public static RowRef<World> CurrentWorld => Svc.PlayerState.CurrentWorld;
    public static RowRef<WorldDCGroupType> HomeDateCenter => HomeWorld.Value.DataCenter;
    public static RowRef<WorldDCGroupType> CurrentDataCenter => CurrentWorld.Value.DataCenter;
    public static RowRef<TerritoryType> Territory => TerritoryType.GetRef(TerritoryId);
    public static RowRef<TerritoryType> HomeAetheryteTerritory => Svc.Data.GetExcelSheet<Aetheryte>().GetRowOrDefault(PlayerState.Instance()->HomeAetheryteId).Value.Territory;
    public static RowRef<ClassJob> ClassJob => Svc.PlayerState.ClassJob;
    public static RowRef<OnlineStatus> OnlineStatus => Object?.OnlineStatus ?? default;
    public static RowRef<ContentFinderCondition> ContentFinderCondition => Lumina.Excel.Sheets.ContentFinderCondition.GetRef(GameMain.Instance()->CurrentContentFinderConditionId);
    #endregion

    #region Excel Values
    public static bool IsInHomeWorld => Available && CurrentWorld.RowId == HomeWorld.RowId;
    public static bool IsInHomeDC => Available && CurrentWorld.Value.DataCenter.RowId == HomeWorld.Value.DataCenter.RowId;
    public static string HomeWorldName => HomeWorld.Value.Name.ToString();
    public static string CurrentWorldName => CurrentWorld.Value.Name.ToString();
    public static string HomeDataCenterName => HomeWorld.Value.DataCenter.Value.Name.ToString();
    public static string CurrentDataCenterName => CurrentWorld.Value.DataCenter.Value.Name.ToString();
    public static uint HomeWorldId => HomeWorld.RowId;
    public static uint CurrentWorldId => CurrentWorld.RowId;
    public static uint JobId => ClassJob.RowId;
    #endregion

    public static Character* Character => (Character*)Object?.Address;
    public static BattleChara* BattleChara => (BattleChara*)Object?.Address;
    public static GameObject* GameObject => (GameObject*)Object?.Address;
    
    public static uint TerritoryId => Svc.ClientState.TerritoryType;
    public static TerritoryIntendedUseEnum TerritoryIntendedUse => (TerritoryIntendedUseEnum)Territory.ValueNullable?.TerritoryIntendedUse.ValueNullable?.RowId;
    public static bool IsInDuty => GameMain.Instance()->CurrentContentFinderConditionId != 0;
    public static bool IsOnIsland => MJIManager.Instance()->IsPlayerInSanctuary;
    public static bool IsInPvP => GameMain.IsInPvPInstance();
    
    public static Job Job => (Job)ClassJob.RowId;
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;
    public static Job GetJob(this IPlayerCharacter pc) => (Job)(pc.ClassJob.RowId);

    public static Vector3 Position => Object?.Position ?? default;
    public static float Rotation => Object?.Rotation ?? default;
    public static bool IsMoving => Available && (AgentMap.Instance()->IsPlayerMoving || IsJumping);
    public static bool IsJumping => Available && (Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61] || Character->IsJumping());
    public static bool Mounted => Svc.Condition[ConditionFlag.Mounted];
    public static bool Mounting => Svc.Condition[ConditionFlag.MountOrOrnamentTransition];
    public static bool CanMount => Territory.Value.Mount && PlayerState.Instance()->NumOwnedMounts > 0;
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
