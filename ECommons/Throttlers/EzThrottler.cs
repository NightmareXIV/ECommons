using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Throttlers
{
    public static class EzThrottler
    {
        static Dictionary<string, long> throttlers = new();

        public static IReadOnlyCollection<string> ThrottleNames => throttlers.Keys;

        public static bool Throttle(string name, int miliseconds = 500, bool rethrottle = false)
        {
            if (!throttlers.ContainsKey(name))
            {
                throttlers[name] = Environment.TickCount64 + miliseconds;
                return true;
            }
            if (Environment.TickCount64 > throttlers[name])
            {
                throttlers[name] = Environment.TickCount64 + miliseconds;
                return true;
            }
            else
            {
                if(rethrottle) throttlers[name] = Environment.TickCount64 + miliseconds;
                return false;
            }
        }

        public static bool Check(string name)
        {
            if (!throttlers.ContainsKey(name)) return true;
            return Environment.TickCount64 > throttlers[name];
        }

        public static long GetRemainingTime(string name, bool allowNegative = false)
        {
            if (!throttlers.ContainsKey(name)) return allowNegative?-Environment.TickCount64:0;
            var ret = throttlers[name] - Environment.TickCount64;
            if (allowNegative)
            {
                return ret;
            }
            else
            {
                return ret > 0 ? ret : 0;
            }
        }

        public static void ImGuiPrintDebugInfo()
        {
            foreach(var x in throttlers)
            {
                ImGuiEx.Text(Check(x.Key)?ImGuiColors.HealerGreen:ImGuiColors.DalamudRed, $"{x.Key}: [{GetRemainingTime(x.Key)}ms remains] ({x.Value})");
            }
        }
    }
}
