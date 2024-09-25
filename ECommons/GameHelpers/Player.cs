using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Numerics;
using GrandCompany = ECommons.ExcelServices.GrandCompany;
#nullable disable

namespace ECommons.GameHelpers;

public static unsafe class Player
{
    public const int MaxLevel = 100;
    public static IPlayerCharacter Object => Svc.ClientState.LocalPlayer;
    public static bool Available => Svc.ClientState.LocalPlayer != null;
    public static bool Interactable => Available && Object.IsTargetable;
    public static ulong CID => Svc.ClientState.LocalContentId;
    public static StatusList Status => Svc.ClientState.LocalPlayer.StatusList;
    public static string Name => Svc.ClientState.LocalPlayer?.Name.ToString();
    public static string NameWithWorld => GetNameWithWorld(Svc.ClientState.LocalPlayer);
    public static string GetNameWithWorld(this IPlayerCharacter pc) => pc == null ? null : (pc.Name.ToString() + "@" + pc.HomeWorld.GameData.Name);

    public static int Level => Svc.ClientState.LocalPlayer?.Level ?? 0;
    public static bool IsLevelSynced => PlayerState.Instance()->IsLevelSynced == 1;
    public static short SyncedLevel => PlayerState.Instance()->SyncedLevel;

    public static bool IsInHomeWorld => Svc.ClientState.LocalPlayer.HomeWorld.Id == Svc.ClientState.LocalPlayer.CurrentWorld.Id;
    public static bool IsInHomeDC => Svc.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Row == Svc.ClientState.LocalPlayer.HomeWorld.GameData.DataCenter.Row;
    public static string HomeWorld => Svc.ClientState.LocalPlayer?.HomeWorld.GameData.Name.ToString();
    public static string CurrentWorld => Svc.ClientState.LocalPlayer?.CurrentWorld.GameData.Name.ToString();
    public static string HomeDataCenter => Svc.Data.GetExcelSheet<World>().GetRow(Svc.ClientState.LocalPlayer.HomeWorld.Id).DataCenter.Value.Name.ToString();
    public static string CurrentDataCenter => Svc.Data.GetExcelSheet<World>().GetRow(Svc.ClientState.LocalPlayer.CurrentWorld.Id).DataCenter.Value.Name.ToString();

    public static Character* Character => (Character*)Svc.ClientState.LocalPlayer.Address;
    public static BattleChara* BattleChara => (BattleChara*)Svc.ClientState.LocalPlayer.Address;
    public static GameObject* GameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;
    [Obsolete("Please use GameObject")]
    public static GameObject* IGameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;

    public static uint Territory => Svc.ClientState.TerritoryType;
    public static TerritoryIntendedUseEnum TerritoryIntendedUse => (TerritoryIntendedUseEnum)Svc.Data.GetExcelSheet<TerritoryType>().GetRow(Territory).TerritoryIntendedUse;
    public static bool IsInDuty => GameMain.Instance()->CurrentContentFinderConditionId != 0;
    public static bool IsOnIsland => MJIManager.Instance()->IsPlayerInSanctuary == 1;

    public static Job Job => GetJob(Svc.ClientState.LocalPlayer);
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;
    public static Job GetJob(this IPlayerCharacter pc) => (Job)pc.ClassJob.Id;

    public static Vector3 Position => Object.Position;
    public static float Rotation => Object.Rotation;
    public static float AnimationLock => *(float*)((nint)ActionManager.Instance() + 8);
    public static bool IsAnimationLocked => AnimationLock > 0;
    public static bool IsMoving => AgentMap.Instance()->IsPlayerMoving == 1;
    public static bool Mounted => Svc.Condition[ConditionFlag.Mounted];
    public static bool Mounting => Svc.Condition[ConditionFlag.Unknown57]; // condition 57 is set while mount up animation is playing
    public static unsafe bool Dismounting => **(byte**)(Svc.ClientState.LocalPlayer.Address + 1432) == 1;
    public static bool Jumping => Svc.Condition[ConditionFlag.Jumping] || Svc.Condition[ConditionFlag.Jumping61];
    public static float DistanceTo(Vector3 other) => Vector3.Distance(Position, other);
    public static float DistanceTo(Vector2 other) => Vector2.Distance(Position.ToVector2(), other);
    public static float DistanceTo(IGameObject other) => Vector3.Distance(Position, other.Position);
}
