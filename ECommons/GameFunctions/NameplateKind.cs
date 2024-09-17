using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.GameFunctions;
public enum NameplateKind
{
    InDutyPCNotInParty = 3  ,
    HostileNotEngaged = 7,
    Dead = 8,
    HostileEngagedSelfUndamaged = 9,
    HostileEngagedOther = 10,
    HostileEngagedSelfDamaged = 11,
    FriendlyBattleNPC = 12,
    OutOfDutyandPartyPC = 22,
    InDutyPCInPartyTank = 27,
    InDutyPCInPartyHealer = 28,
    InDutyPCInPartyDPS = 29,
}
