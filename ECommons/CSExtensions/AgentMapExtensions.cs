using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.CSExtensions;
public unsafe static class AgentMapExtensions
{
    public static bool IsFlagMarkerSet(this AgentMap agent)
    {
        return agent.FlagMarkerCount > 0;
    }

    public static FlagMapMarker FlagMapMarker(this AgentMap agent)
    {
        return agent.FlagMapMarkers[0];
    }
}
