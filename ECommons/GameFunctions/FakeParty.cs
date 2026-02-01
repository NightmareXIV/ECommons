using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers.LegacyPlayer;
using System.Collections.Generic;

namespace ECommons.GameFunctions;

public static class FakeParty
{
    /// <summary>
    /// Must be used ONLY on Framework update thread.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<IPlayerCharacter> Get()
    {
        if(Svc.Condition[ConditionFlag.DutyRecorderPlayback])
        {
            foreach(var x in Svc.Objects)
            {
                if(x is IPlayerCharacter pc)
                {
                    yield return pc;
                }
            }
        }
        else
        {
            if(Svc.Party.Count > 0)
            {
                foreach(var x in Svc.Party)
                {
                    if(x.GameObject is IPlayerCharacter pc)
                    {
                        yield return pc;
                    }
                }
            }
            else
            {
                yield return Player.Object;
            }
        }
    }
}
