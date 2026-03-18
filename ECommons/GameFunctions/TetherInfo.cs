using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.VfxContainer;

namespace ECommons.GameFunctions;

public class TetherInfo
{
    public uint Id => RawInfo.Id;
    public bool IsSource { get; init; }

    public Tether RawInfo { get; init; }

    public uint PairId { get; init; }
    public IGameObject? Pair => Svc.Objects.FirstOrDefault(x => x.ObjectId == PairId);

    public TetherInfo()
    {
    }

    public TetherInfo(Tether rawInfo, uint pairId, bool isSource)
    {
        RawInfo = rawInfo;
        PairId = pairId;
        IsSource = isSource;
    }

    public override string ToString()
    {
        return $"[Tether {Id} with {Pair?.Name} (D={Pair?.DataId}, NID={(Pair is ICharacter c?c.NameId.ToString():"")}) ({(IsSource?"Source":"Target")})]";
    }
}
