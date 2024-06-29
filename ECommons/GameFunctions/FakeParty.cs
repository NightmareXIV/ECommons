using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using System.Collections.Generic;

namespace ECommons.GameFunctions;

public static class FakeParty
{
    public static IEnumerable<IPlayerCharacter> Get()
    {
        if (Svc.Condition[ConditionFlag.DutyRecorderPlayback])
        {
            foreach (var x in Svc.Objects)
            {
                if (x is IPlayerCharacter pc)
                {
                    yield return pc;
                }
            }
        }
        else
        {
            foreach(var x in Svc.Party)
            {
                if(x.GameObject is IPlayerCharacter pc)
                {
                    yield return pc;
                }
            }
        }
    }
}
