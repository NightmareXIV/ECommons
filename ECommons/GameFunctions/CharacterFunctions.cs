using Dalamud.Game.ClientState.Objects.Types;
using ECommons.MathHelpers;
using System;
using System.Collections.Generic;

namespace ECommons.GameFunctions;

public static unsafe class CharacterFunctions
{
    public static ushort GetVFXId(void* VfxData)
    {
        if (VfxData == null) return 0;
        return *(ushort*)((IntPtr)(VfxData) + 8);
    }

    public static FFXIVClientStructs.FFXIV.Client.Game.Character.Character* Struct(this ICharacter o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)o.Address;
    }

    public static FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct(this IBattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)o.Address;
    }

    public static FFXIVClientStructs.FFXIV.Client.Game.Character.Character* Character(this IBattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)o.Address;
    }

    public static FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* GameObject(this IBattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)o.Address;
    }

    public static FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* IGameObject(this ICharacter o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)o.Address;
    }

    public static bool IsCharacterVisible(this ICharacter chr)
    {
        var v = (IntPtr)(((FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)chr.Address)->GameObject.DrawObject);
        if (v == IntPtr.Zero) return false;
        return Bitmask.IsBitSet(*(byte*)(v + 136), 0);
    }

    public static byte GetTransformationID(this ICharacter chr)
    {
        return *(byte*)(chr.Address + 3120);
    }

    public static bool IsInWater(this ICharacter chr)
    {
        return *(byte*)(chr.Address + 528 + 940) == 1;
    }

    public static CombatRole GetRole(this ICharacter c)
    {
        if (c.ClassJob.GameData?.Role == 1) return CombatRole.Tank;
        if (c.ClassJob.GameData?.Role == 2) return CombatRole.DPS;
        if (c.ClassJob.GameData?.Role == 3) return CombatRole.DPS;
        if (c.ClassJob.GameData?.Role == 4) return CombatRole.Healer;
        return CombatRole.NonCombat;
    }

    public static bool IsCasting(this IBattleChara c, uint spellId = 0)
    {
        return c.IsCasting && (spellId == 0 || c.CastActionId.EqualsAny(spellId));
    }

    public static bool IsCasting(this IBattleChara c, params uint[] spellId)
    {
        return c.IsCasting && c.CastActionId.EqualsAny(spellId);
    }

    public static bool IsCasting(this IBattleChara c, IEnumerable<uint> spellId)
    {
        return c.IsCasting && c.CastActionId.EqualsAny(spellId);
    }
}
