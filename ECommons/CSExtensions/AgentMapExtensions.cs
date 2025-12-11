using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.CSExtensions;
public unsafe static class AgentMapExtensions
{
    extension(ref AgentMap agent)
    {
        public bool IsFlagMarkerSet => agent.FlagMarkerCount > 0;
        public FlagMapMarker FlagMapMarker => agent.FlagMapMarkers[0];
    }
}
