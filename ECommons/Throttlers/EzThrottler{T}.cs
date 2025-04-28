using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using System;
using System.Collections.Generic;

namespace ECommons.Throttlers;
#nullable disable

public class EzThrottler<T>
{
    private Dictionary<T, long> Throttlers = [];
    public IReadOnlyCollection<T> ThrottleNames => Throttlers.Keys;

    public bool Throttle(T name, TimeSpan ts, bool reThrottle = false) => Throttle(name, (int)ts.TotalMilliseconds, reThrottle);

    [Obsolete("The 'rethrottle' argument will be removed in a future version. It will behave as if 'false' is always passed. Please update your code accordingly.")]
    public bool Throttle(T name, int miliseconds = 500, bool rethrottle = false)
    {
        _ = rethrottle;
        return Throttle(name, miliseconds);
    }

    public bool Throttle(T name, int miliseconds = 500)
    {
        Update();

        if(!Throttlers.ContainsKey(name))
        {
            Throttlers[name] = Environment.TickCount64 + miliseconds;
            return true;
        }
        if(Environment.TickCount64 > Throttlers[name])
        {
            Throttlers[name] = Environment.TickCount64 + miliseconds;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset(T name)
    {
        Throttlers.Remove(name);
    }

    public bool Check(T name)
    {
        if(!Throttlers.ContainsKey(name)) return true;
        return Environment.TickCount64 > Throttlers[name];
    }

    public long GetRemainingTime(T name, bool allowNegative = false)
    {
        if(!Throttlers.ContainsKey(name)) return allowNegative ? -Environment.TickCount64 : 0;
        var ret = Throttlers[name] - Environment.TickCount64;
        if(allowNegative)
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
        foreach(var x in Throttlers)
        {
            ImGuiEx.Text(Check(x.Key) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, $"{x.Key}: [{GetRemainingTime(x.Key)}ms remains] ({x.Value})");
        }
    }

    private void Update()
    {
        // Remove expired throttlers
        foreach(var x in Throttlers)
        {
            if(x.Value < Environment.TickCount64)
            {
                Throttlers.Remove(x.Key);
            }
        }
    }
}
