using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.Logging;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ECommons.GameFunctions;
#nullable disable

public static unsafe class ObjectFunctions
{
    public delegate byte GetNameplateColorDelegate(nint ptr);
    public static GetNameplateColorDelegate GetNameplateColor;

    public static string GetNameplateColorSig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 35 ?? ?? ?? ?? 48 8B F9";

    public static FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* Struct(this IGameObject o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)o.Address;
    }

    internal static void Init()
    {
        GetNameplateColor ??= EzDelegate.Get<GetNameplateColorDelegate>(GetNameplateColorSig);
    }

    [Obsolete($"Use {nameof(IGameObject.IsTargetable)}")]
    public static bool IsTargetable(this IGameObject o)
    {
        return o.Struct()->GetIsTargetable();
    }

    public static bool IsHostile(this IGameObject a)
    {
        GetNameplateColor ??= EzDelegate.Get<GetNameplateColorDelegate>(GetNameplateColorSig);
        var plateType = GetNameplateColor(a.Address);
        //7: yellow, can be attacked, not engaged
        //8: dead
        //9: red, engaged with your party
        //11: orange, aggroed to your party but not attacked yet
        //10: purple, engaged with other party
        return plateType == 7 || plateType == 9 || plateType == 11 || plateType == 10;
    }

    public static int GetAttackableEnemyCountAroundPoint(Vector3 point, float radius)
    {
        int num = 0;
        foreach(var o in Svc.Objects)
        {
            if(o is IBattleNpc)
            {
                var oStruct = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)o.Address;
                if(oStruct->GetIsTargetable() && o.IsHostile()
                    && Vector3.Distance(point, o.Position) <= radius + o.HitboxRadius)
                {
                    num++;
                }
            }
        }
        return num;
    }

    public static bool TryGetPartyMemberObjectByObjectId(uint objectId, out IGameObject partyMemberObject)
    {
        if (objectId == Svc.ClientState.LocalPlayer?.GameObjectId)
        {
            partyMemberObject = Svc.ClientState.LocalPlayer;
            return true;
        }
        foreach (var p in Svc.Party)
        {
            if (p.GameObject?.GameObjectId == objectId)
            {
                partyMemberObject = p.GameObject;
                return true;
            }
        }
        partyMemberObject = default;
        return false;
    }

    public static bool TryGetPartyMemberObjectByAddress(IntPtr address, out IGameObject partyMemberObject)
    {
        if (address == Svc.ClientState.LocalPlayer?.Address)
        {
            partyMemberObject = Svc.ClientState.LocalPlayer;
            return true;
        }
        foreach (var p in Svc.Party)
        {
            if (p.GameObject?.Address == address)
            {
                partyMemberObject = p.GameObject;
                return true;
            }
        }
        partyMemberObject = default;
        return false;
    }
}
