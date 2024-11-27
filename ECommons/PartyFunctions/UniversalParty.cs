using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Memory;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommons.PartyFunctions;

public static unsafe class UniversalParty
{
    public static bool IsCrossWorldParty => Svc.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    public static bool IsAlliance => IsCrossWorldParty && InfoProxyCrossRealm.Instance()->IsInAllianceRaid != 0;

    public static int Length => Members.Count;
    public static int LengthPlayback => MembersPlayback.Count;

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
                    ClassJob = Player.Job,
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
                                ClassJob = (ExcelServices.Job)x.ClassJobId,
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
                            ClassJob = (ExcelServices.Job)x.ClassJob.RowId,
                        });
                    }
                }
            }
            return span;
        }
    }

    public static List<UniversalPartyMember> MembersPlayback
    {
        get
        {
            if(!Player.Available) return [];
            if(Svc.Condition[ConditionFlag.DutyRecorderPlayback])
            {
                var ret = new List<UniversalPartyMember>
                {
                    new()
                    {
                        Name = Player.Name,
                        HomeWorld = Player.Object.HomeWorld,
                        CurrentWorld = Player.Object.CurrentWorld,
                        GameObjectInternal = Player.Object,
                        ContentID = Player.CID,
                        ClassJob = Player.Job,
                    }
                };
                foreach(var x in Svc.Objects.OfType<IPlayerCharacter>())
                {
                    if(!x.AddressEquals(Player.Object))
                    {
                        ret.Add(new()
                        {
                            Name = x.Name.ToString(),
                            HomeWorld = x.HomeWorld,
                            CurrentWorld = Player.Object!.CurrentWorld,
                            GameObjectInternal = x,
                            ContentID = x.Struct()->ContentId,
                            ClassJob = (ExcelServices.Job)x.ClassJob.RowId,
                        });
                    }
                }
                return ret;
            }
            else
            {
                return Members;
            }
        }
    }
}
