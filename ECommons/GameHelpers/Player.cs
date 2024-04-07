﻿using ECommons.ExcelServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
#nullable disable

namespace ECommons.GameHelpers;

public unsafe static class Player
{
    public static PlayerCharacter Object => Svc.ClientState.LocalPlayer;
    public static bool Available => Svc.ClientState.LocalPlayer != null;
    public static bool Interactable => Available && Object.IsTargetable;
    public static ulong CID => Svc.ClientState.LocalContentId;
    public static StatusList Status => Svc.ClientState.LocalPlayer.StatusList;
    public static string Name => Svc.ClientState.LocalPlayer?.Name.ToString();
    public static string NameWithWorld => GetNameWithWorld(Svc.ClientState.LocalPlayer);
    public static int Level => Svc.ClientState.LocalPlayer?.Level ?? 0;
    public static bool IsInHomeWorld => Svc.ClientState.LocalPlayer.HomeWorld.Id == Svc.ClientState.LocalPlayer.CurrentWorld.Id;
    public static string HomeWorld => Svc.ClientState.LocalPlayer?.HomeWorld.GameData.Name.ToString();
    public static string CurrentWorld => Svc.ClientState.LocalPlayer?.CurrentWorld.GameData.Name.ToString();
    public static Character* Character => (Character*)Svc.ClientState.LocalPlayer.Address;
    public static BattleChara* BattleChara => (BattleChara*)Svc.ClientState.LocalPlayer.Address;
    public static GameObject* GameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;
    public static uint Territory => Svc.ClientState.TerritoryType;
    public static Job Job => GetJob(Svc.ClientState.LocalPlayer);
    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.Instance()->GrandCompany;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNameWithWorld(this PlayerCharacter pc) => pc == null?null:(pc.Name.ToString() + "@" + pc.HomeWorld.GameData.Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Job GetJob(this PlayerCharacter pc) => (Job)pc.ClassJob.Id;
}
