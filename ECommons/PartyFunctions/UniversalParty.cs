using Dalamud.Game.ClientState.Conditions;
using Dalamud.Memory;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Collections.Generic;

namespace ECommons.PartyFunctions;

public unsafe static class UniversalParty
{
    public static bool IsCrossWorldParty => Svc.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    public static bool IsAlliance => IsCrossWorldParty && InfoProxyCrossRealm.Instance()->IsInAllianceRaid != 0;

    public static int Length => Members.Count;

    public static List<UniversalPartyMember> Members
    {
        get
        {
            if (!Player.Available) return [];
            var span = new List<UniversalPartyMember>
            {
                new()
                {
                    Name = Player.Name,
                    HomeWorld = new(Player.Object.HomeWorld),
                    CurrentWorld = new(Player.Object.CurrentWorld),
                    GameObjectInternal = Player.Object
                }
            };
            if (IsCrossWorldParty)
            {
                var proxy = InfoProxyCrossRealm.Instance();
                for (int i = 0; i < proxy->GroupCount; i++)
                {
                    var group = proxy->CrossRealmGroupArraySpan[i];
                    for (int c = 0; c < group.GroupMemberCount; c++)
                    {
                        var x = group.GroupMembersSpan[c];
                        var name = MemoryHelper.ReadStringNullTerminated((nint)x.Name);
                        if (name != Player.Name && x.HomeWorld != Player.Object.HomeWorld.Id)
                        {
                            span.Add(new()
                            {
                                Name = name,
                                HomeWorld = new((uint)x.HomeWorld),
                                CurrentWorld = new((uint)x.CurrentWorld),
                            }) ;
                        }
                    }
                }
            }
            else
            {
                foreach(var x in Svc.Party)
                {
                    if (x.GameObject?.Address != Player.Object?.Address)
                    {
                        span.Add(new()
                        {
                            Name = x.Name.ToString(),
                            HomeWorld = new(x.World),
                            CurrentWorld = new(Player.Object!.CurrentWorld),
                            GameObjectInternal = x.GameObject
                        });
                    }
                }
            }
            return span;
        }
    }
}
