using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.DalamudServices.Legacy;

public static class IClientStateExtensions
{
    extension(IClientState value)
    {
        public IPlayerCharacter LocalPlayer => Svc.Objects.LocalPlayer;
    }
}
