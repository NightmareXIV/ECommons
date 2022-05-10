using Dalamud.Game.ClientState.Objects.Types;
using ECommons.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.GameFunctions
{
    public static unsafe class CharacterFunctions
    {
        public static bool IsCharacterVisible(this Character chr)
        {
            var v = (IntPtr)(((FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)chr.Address)->GameObject.DrawObject);
            if (v == IntPtr.Zero) return false;
            return Bitmask.IsBitSet(*(byte*)(v + 136), 0);
        }

        public static int GetModelId(this Character a)
        {
            return *(int*)(a.Address + 0x01B4);
        }

        public static CombatRole GetRole(this Character c)
        {
            if (c.ClassJob.GameData.Role == 1) return CombatRole.Tank;
            if (c.ClassJob.GameData.Role == 2) return CombatRole.DPS;
            if (c.ClassJob.GameData.Role == 3) return CombatRole.DPS;
            if (c.ClassJob.GameData.Role == 4) return CombatRole.Healer;
            return CombatRole.NonCombat;
        }
    }
}
