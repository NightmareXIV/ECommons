using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using System;
using System.Collections.Generic;

namespace ECommons.Throttlers;
#nullable disable

public class EzThrottler<T>
{
    Dictionary<T, long> Throttlers = new();
    public IReadOnlyCollection<T> ThrottleNames => Throttlers.Keys;
    public bool Throttle(T name, int miliseconds = 500, bool rethrottle = false)
    {
        if (!Throttlers.ContainsKey(name))
        {
            Throttlers[name] = Environment.TickCount64 + miliseconds;
            return true;
        }
        if (Environment.TickCount64 > Throttlers[name])
        {
            Throttlers[name] = Environment.TickCount64 + miliseconds;
            return true;
        }
        else
        {
            if (rethrottle) Throttlers[name] = Environment.TickCount64 + miliseconds;
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
        return Environment.TickCount64 > Throttlers[name];
    }

    public long GetRemainingTime(T name, bool allowNegative = false)
    {
        if (!Throttlers.ContainsKey(name)) return allowNegative ? -Environment.TickCount64 : 0;
        var ret = Throttlers[name] - Environment.TickCount64;
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
            ImGuiEx.Text(Check(x.Key) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, $"{x.Key}: [{GetRemainingTime(x.Key)}ms remains] ({x.Value})");
        }
    }
}
