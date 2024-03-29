using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;

namespace ECommons.PartyFunctions;

public class UniversalPartyMember
#nullable disable
{
    public string Name { get; init; }
    public ExcelResolver<World> HomeWorld { get; init; }
    public ExcelResolver<World> CurrentWorld { get; init; }
    public string NameWithWorld => $"{Name}@{HomeWorld.GameData.Name}";

    internal GameObject GameObjectInternal = null;
    public GameObject GameObject
    {
        get
        {
            if (UniversalParty.IsCrossWorldParty)
            {
                return Svc.Objects.FirstOrDefault(x => x is PlayerCharacter pc && pc.HomeWorld.Id == this.HomeWorld.Id && x.Name.ToString() == this.Name);
            }
            else
            {
                return GameObjectInternal;
            }
        }
    }
}
