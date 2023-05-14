using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Throttlers
{
    public static class FrameThrottler
    {
        static Dictionary<string, long> throttlers = new();
        static long SFrameCount => (long) Svc.PluginInterface.UiBuilder.FrameCount;

        public static IReadOnlyCollection<string> ThrottleNames => throttlers.Keys;

        public static bool Throttle(string name, int frames = 60, bool rethrottle = false)
        {
            if (!throttlers.ContainsKey(name))
            {
                throttlers[name] = SFrameCount + frames;
                return true;
            }
            if (SFrameCount > throttlers[name])
            {
                throttlers[name] = SFrameCount + frames;
                return true;
            }
            else
            {
                if (rethrottle) throttlers[name] = SFrameCount + frames;
                return false;
            }
        }

        public static bool Check(string name)
        {
            if (!throttlers.ContainsKey(name)) return true;
            return SFrameCount > throttlers[name];
        }

        public static long GetRemainingTime(string name, bool allowNegative = false)
        {
            if (!throttlers.ContainsKey(name)) return allowNegative ? -SFrameCount : 0;
            var ret = throttlers[name] - SFrameCount;
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
            foreach (var x in throttlers)
            {
                ImGuiEx.Text(Check(x.Key) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, $"{x.Key}: [{GetRemainingTime(x.Key)} frames remains] ({x.Value})");
            }
        }
    }
}
