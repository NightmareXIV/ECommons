using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using System.Collections.Generic;

namespace ECommons.Throttlers;
#nullable disable

public class FrameThrottler<T>
{
    Dictionary<T, long> Throttlers = new();
    long SFrameCount => (long)Svc.PluginInterface.UiBuilder.FrameCount;

    public IReadOnlyCollection<T> ThrottleNames => Throttlers.Keys;

    public bool Throttle(T name, int frames = 60, bool rethrottle = false)
    {
        if (!Throttlers.ContainsKey(name))
        {
            Throttlers[name] = SFrameCount + frames;
            return true;
        }
        if (SFrameCount > Throttlers[name])
        {
            Throttlers[name] = SFrameCount + frames;
            return true;
        }
        else
        {
            if (rethrottle) Throttlers[name] = SFrameCount + frames;
            return false;
        }
    }

    public void Reset(T name)
    {
        Throttlers.Remove(name);
    }

    public bool Check(T name)
    {
        if (!Throttlers.ContainsKey(name)) return true;
        return SFrameCount > Throttlers[name];
    }

    public long GetRemainingTime(T name, bool allowNegative = false)
    {
        if (!Throttlers.ContainsKey(name)) return allowNegative ? -SFrameCount : 0;
        var ret = Throttlers[name] - SFrameCount;
        if (allowNegative)
        {
            return ret;
        }
        else
        {
            return ret > 0 ? ret : 0;
        }
    }

    public void ImGuiPrintDebugInfo()
    {
        foreach (var x in Throttlers)
        {
            ImGuiEx.Text(Check(x.Key) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, $"{x.Key}: [{GetRemainingTime(x.Key)} frames remains] ({x.Value})");
        }
    }
}
