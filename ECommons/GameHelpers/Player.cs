using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using System;
using System.Numerics;
using Aetheryte = Lumina.Excel.Sheets.Aetheryte;
using GrandCompany = ECommons.ExcelServices.GrandCompany;
#nullable disable

namespace ECommons.GameHelpers;

public static unsafe class Player
{
    public static readonly Number MaxLevel = 100;
    public static IPlayerCharacter Object => Svc.ClientState.LocalPlayer;
    public static bool Available => Svc.ClientState.LocalPlayer != null;
    public static bool AvailableThreadSafe => GameObjectManager.Instance()->Objects.IndexSorted[0].Value != null;
    public static bool Interactable => Available && Object.IsTargetable;
    public static bool IsBusy => GenericHelpers.IsOccupied() || Object.IsCasting || IsMoving || IsAnimationLocked || Svc.Condition[ConditionFlag.InCombat];
    public static ulong CID => Svc.ClientState.LocalContentId;
    public static StatusList Status => Svc.ClientState.LocalPlayer?.StatusList;
    public static string Name => Svc.ClientState.LocalPlayer?.Name.ToString();
    public static string NameWithWorld => GetNameWithWorld(Svc.ClientState.LocalPlayer);
    public static string GetNameWithWorld(this IPlayerCharacter pc) => pc == null ? null : (pc.Name.ToString() + "@" + pc.HomeWorld.ValueNullable?.Name.ToString());

    public static int Level => Svc.ClientState.LocalPlayer?.Level ?? 0;
    public static bool IsLevelSynced => PlayerState.Instance()->IsLevelSynced == 1;
    public static int SyncedLevel => PlayerState.Instance()->SyncedLevel;
    public static int UnsyncedLevel => GetUnsyncedLevel(GetJob(Object));
    public static int GetUnsyncedLevel(Job job) => PlayerState.Instance()->ClassJobLevels[Svc.Data.GetExcelSheet<ClassJob>().GetRowOrDefault((uint)job).Value.ExpArrayIndex];

    public static bool IsInHomeWorld => !Player.Available ? false : Svc.ClientState.LocalPlayer.HomeWorld.RowId == Svc.ClientState.LocalPlayer.CurrentWorld.RowId;
    public static bool IsInHomeDC => !Player.Available ? false : Svc.ClientState.LocalPlayer.CurrentWorld.Value.DataCenter.RowId == Svc.ClientState.LocalPlayer.HomeWorld.Value.DataCenter.RowId;
    public static string HomeWorld => Svc.ClientState.LocalPlayer?.HomeWorld.Value.Name.ToString();
    public static string CurrentWorld => Svc.ClientState.LocalPlayer?.CurrentWorld.Value.Name.ToString();
    public static string HomeDataCenter => Svc.Data.GetExcelSheet<World>().GetRowOrDefault(HomeWorldId)?.DataCenter.ValueNullable?.Name.ToString();
    public static string CurrentDataCenter => Svc.Data.GetExcelSheet<World>().GetRowOrDefault(CurrentWorldId)?.DataCenter.ValueNullable?.Name.ToString();

    public static Character* Character => (Character*)Svc.ClientState.LocalPlayer.Address;
    public static BattleChara* BattleChara => (BattleChara*)Svc.ClientState.LocalPlayer.Address;
    public static GameObject* GameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;

    public static uint Territory => Svc.ClientState.TerritoryType;
    public static TerritoryIntendedUseEnum TerritoryIntendedUse => (TerritoryIntendedUseEnum)(Svc.Data.GetExcelSheet<TerritoryType>().GetRowOrDefault(Territory)?.TerritoryIntendedUse.ValueNullable?.RowId ?? default);
    public static uint HomeAetheryteTerritory => Svc.Data.GetExcelSheet<Aetheryte>().GetRowOrDefault(PlayerState.Instance()->HomeAetheryteId).Value.Territory.RowId;
    public static bool IsInDuty => GameMain.Instance()->CurrentContentFinderConditionId != 0;
    public static bool IsOnIsland => MJIManager.Instance()->IsPlayerInSanctuary == 1;
    public static bool IsInPvP => GameMain.IsInPvPInstance();

    public static Job Job => GetJob(Svc.ClientState.LocalPlayer);
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;
    public static Job GetJob(this IPlayerCharacter pc) => (Job)(pc?.ClassJob.RowId ?? 0);

    public static uint HomeWorldId => Player.Object?.HomeWorld.RowId ?? 0;
    public static uint CurrentWorldId => Player.Object?.CurrentWorld.RowId ?? 0;
    public static uint JobId => Player.Object?.ClassJob.RowId ?? 0;
    public static uint OnlineStatus => Player.Object?.OnlineStatus.RowId ?? 0;

    public static Vector3 Position => Available ? Object.Position : Vector3.Zero;
    public static float Rotation => Available ? Object.Rotation : 0;
    public static bool IsMoving => Available && (AgentMap.Instance()->IsPlayerMoving || IsJumping);
    public static bool IsJumping => Available && (Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61] || Character->IsJumping());
    public static bool Mounted => Svc.Condition[ConditionFlag.Mounted];
    public static bool Mounting => Svc.Condition[ConditionFlag.MountOrOrnamentTransition];
    public static bool CanMount => Svc.Data.GetExcelSheet<TerritoryType>().GetRow(Territory).Mount && PlayerState.Instance()->NumOwnedMounts > 0;
    public static bool CanFly => Control.CanFly;

    public static float AnimationLock => *(float*)((nint)ActionManager.Instance() + 8);
    public static bool IsAnimationLocked => AnimationLock > 0;
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
