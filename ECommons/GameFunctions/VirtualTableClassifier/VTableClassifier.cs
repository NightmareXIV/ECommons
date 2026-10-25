using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

namespace ECommons.GameFunctions.VirtualTableClassifier;
#nullable enable

public static class VTableClassifier
{
    public static unsafe VObjectKind Classify(void* gameObject)
    {
        if(gameObject == null) return VObjectKind.Null;
        var ptr = (GameObject*)gameObject;
        var addr = (nint)ptr->VirtualTable;
        if(addr == (nint)GameObject.StaticVirtualTablePointer) return VObjectKind.GameObject;
        if(addr == (nint)Character.StaticVirtualTablePointer) return VObjectKind.Character;
        if(addr == (nint)BattleChara.StaticVirtualTablePointer) return VObjectKind.BattleChara;
        return VObjectKind.Unknown;
    }

    public static unsafe VObjectKind Classify(this ref GameObject obj)
    {
        fixed(GameObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref Character obj)
    {
        fixed(Character* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref BattleChara obj)
    {
        fixed(BattleChara* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref EventObject obj)
    {
        fixed(EventObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref AreaObject obj)
    {
        fixed(AreaObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref Aetheryte obj)
    {
        fixed(Aetheryte* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref GatheringPointObject obj)
    {
        fixed(GatheringPointObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref HousingObject obj)
    {
        fixed(HousingObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref HousingEventObject obj)
    {
        fixed(HousingEventObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref HousingCombinedObject obj)
    {
        fixed(HousingCombinedObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref ReactionEventObject obj)
    {
        fixed(ReactionEventObject* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this ref Treasure obj)
    {
        fixed(Treasure* ptr = &obj) return Classify(ptr);
    }

    public static unsafe VObjectKind Classify(this IGameObject? obj)
    {
        if(obj == null) return VObjectKind.Null;
        return Classify((void*)obj.Address);
    }

    public static bool IsBattleChara(this IGameObject? obj)
    {
        return obj.Classify() == VObjectKind.BattleChara;
    }

    public static bool IsBattleChara(this IGameObject? obj, [NotNullWhen(true)]out IBattleChara? chr)
    {
        if(obj.IsBattleChara() && obj is IBattleChara b)
        {
            chr = b;
            return true;
        }
        chr = default;
        return false;
    }

    public static bool IsBattleNpc(this IGameObject? obj)
    {
        return obj.Classify() == VObjectKind.BattleChara && obj.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc;
    }

    public static bool IsBattleNpc(this IGameObject? obj, [NotNullWhen(true)]out IBattleNpc? chr)
    {
        if(obj.IsBattleNpc() && obj is IBattleNpc b)
        {
            chr = b;
            return true;
        }
        chr = default;
        return false;
    }

    public static IBattleNpc? AsBattleNpc(this IGameObject? obj)
    {
        if(obj.IsBattleNpc(out var b))
        {
            return b;
        }
        return null;
    }

    public static IBattleChara? AsBattleChara(this IGameObject? obj)
    {
        if(obj.IsBattleChara(out var b))
        {
            return b;
        }
        return null;
    }

    public static IEnumerable<IBattleNpc> OfTypeIBattleNpc<T>(this IEnumerable<T> objects) where T : IGameObject
    {
        foreach(var x in objects)
        {
            if(x.IsBattleNpc(out var b)) yield return b;
        }
    }

    public static IEnumerable<IBattleChara> OfTypeIBattleChara<T>(this IEnumerable<T> objects) where T : IGameObject
    {
        foreach(var x in objects)
        {
            if(x.IsBattleChara(out var b)) yield return b;
        }
    }
}
