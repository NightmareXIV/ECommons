using Dalamud.Game.ClientState.Conditions;
using Dalamud.Memory;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Collections.Generic;
using System.Text;

namespace ECommons.PartyFunctions;

public static unsafe class UniversalParty
{
    public static bool IsCrossWorldParty => Svc.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    public static bool IsAlliance => IsCrossWorldParty && InfoProxyCrossRealm.Instance()->IsInAllianceRaid != 0;

    public static int Length => Members.Count;

    public static List<UniversalPartyMember> Members
    {
        get
        {
            if(!Player.Available) return [];
            var span = new List<UniversalPartyMember>
            {
                new()
                {
                    Name = Player.Name,
                    HomeWorld = Player.Object.HomeWorld,
                    CurrentWorld = Player.Object.CurrentWorld,
                    GameObjectInternal = Player.Object,
                    ContentID = Player.CID,
                }
            };
            if(IsCrossWorldParty)
            {
                var proxy = InfoProxyCrossRealm.Instance();
                for(var i = 0; i < proxy->GroupCount; i++)
                {
                    var group = proxy->CrossRealmGroups[i];
                    for(var c = 0; c < group.GroupMemberCount; c++)
                    {
                        var x = group.GroupMembers[c];
                        var name = GenericHelpers.Read(x.Name);
                        if(!(name == Player.Name && x.HomeWorld == Player.Object.HomeWorld.RowId))
                        {
                            span.Add(new()
                            {
                                Name = name,
                                HomeWorld = new(Svc.Data.Excel, (uint)x.HomeWorld),
                                CurrentWorld = new(Svc.Data.Excel, (uint)x.CurrentWorld),
                                ContentID = x.ContentId,
                            });
                        }
                    }
                }
            }
            else
            {
                foreach(var x in Svc.Party)
                {
                    if(x.GameObject?.Address != Player.Object?.Address)
                    {
                        span.Add(new()
                        {
                            Name = x.Name.ToString(),
                            HomeWorld = x.World,
                            CurrentWorld = Player.Object!.CurrentWorld,
                            GameObjectInternal = x.GameObject,
                            ContentID = (ulong)x.ContentId,
                        });
                    }
                }
            }
            return span;
        }
    }
}
