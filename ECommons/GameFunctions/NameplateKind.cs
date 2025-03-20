using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.GameFunctions;
public enum NameplateKind
{
    /// <summary>
    /// Represents the player's own character.
    /// </summary>
    PlayerCharacterSelf = 1,

    /// <summary>
    /// Represents another party member in duty (job agnostic, likely only applicable in PVP).
    /// </summary>
    InDutyPartyMember = 2,

    /// <summary>
    /// Represents another player character in duty but not in the party.
    /// </summary>
    InDutyPCNotInParty = 3,

    /// <summary>
    /// Represents an enemy player character in Maelstrom PVP.
    /// </summary>
    EnemyMalestromPVPPC = 4,

    /// <summary>
    /// Represents an enemy player character in Adder PVP.
    /// </summary>
    EnemyAdderPVPPC = 5,

    /// <summary>
    /// Represents an enemy player character in Flames PVP.
    /// </summary>
    EnemyFlamesPVPPC = 6,

    /// <summary>
    /// Represents a hostile character that is not engaged.
    /// </summary>
    HostileNotEngaged = 7,

    /// <summary>
    /// Represents a dead character.
    /// </summary>
    Dead = 8,

    /// <summary>
    /// Represents a hostile character engaged with the player and damaged.
    /// </summary>
    HostileEngagedSelfDamaged = 9,

    /// <summary>
    /// Represents a hostile character engaged with another player.
    /// </summary>
    HostileEngagedOther = 10,

    /// <summary>
    /// Represents a hostile character engaged with the player but undamaged.
    /// </summary>
    HostileEngagedSelfUndamaged = 11,

    /// <summary>
    /// Represents a friendly battle NPC.
    /// </summary>
    FriendlyBattleNPC = 12,

    /// <summary>
    /// Represents the player's own chocobo.
    /// </summary>
    PlayerCharacterChocobo = 15,

    /// <summary>
    /// Represents another player's chocobo.
    /// </summary>
    OtherPlayerCharacterChocobo = 17,

    /// <summary>
    /// Represents another player character in another alliance.
    /// </summary>
    OtherAlliancePlayerCharacter = 20,

    /// <summary>
    /// Represents any dead player character.
    /// </summary>
    AnyPlayerCharacterDead = 21,

    /// <summary>
    /// Represents another player character out of duty and party.
    /// </summary>
    OutOfDutyandPartyPC = 22,

    /// <summary>
    /// Represents another tank role player character in duty and in party.
    /// </summary>
    InDutyPCInPartyTank = 27,

    /// <summary>
    /// Represents another healer role player character in duty and in party.
    /// </summary>
    InDutyPCInPartyHealer = 28,

    /// <summary>
    /// Represents another DPS role player character in duty and in party.
    /// </summary>
    InDutyPCInPartyDPS = 29,
}
