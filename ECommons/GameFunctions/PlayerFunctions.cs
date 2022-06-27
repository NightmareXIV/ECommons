using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.GameFunctions
{
    public unsafe static class PlayerFunctions
    {
        public static bool TryGetPlaceholder(this GameObject pc, out int number)
        {
            for(var i = 1; i <= 8; i++)
            {
                if(pc.Address == (IntPtr)Framework.Instance()->GetUiModule()->GetPronounModule()->ResolvePlaceholder($"<number>", 0, 0))
                {
                    number = i;
                    return true;
                }
            }
            number = default;
            return false;
        }
    }
}
