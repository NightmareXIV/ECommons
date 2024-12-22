using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System.Linq;

namespace ECommons.PartyFunctions;

public class UniversalPartyMember
#nullable disable
{
    public string Name { get; init; }
    public RowRef<World> HomeWorld { get; init; }
    public RowRef<World> CurrentWorld { get; init; }
    public string NameWithWorld => $"{Name}@{HomeWorld.ValueNullable?.Name}";
    public ulong ContentID { get; init; }
    public Job ClassJob { get; init; }

    internal IGameObject GameObjectInternal = null;
    public IGameObject IGameObject
    {
        get
        {
            if(UniversalParty.IsCrossWorldParty)
            {
                return Svc.Objects.FirstOrDefault(x => x is IPlayerCharacter pc && pc.HomeWorld.RowId == HomeWorld.RowId && x.Name.ToString() == Name);
            }
            else
            {
                return GameObjectInternal;
            }
        }
    }
}
