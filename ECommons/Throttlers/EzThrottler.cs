using System.Collections.Generic;

namespace ECommons.Throttlers;

public static class EzThrottler
{
    internal static EzThrottler<string> Throttler = new();

    public static IReadOnlyCollection<string> ThrottleNames => Throttler.ThrottleNames;

    public static bool Throttle(string name, int miliseconds = 500, bool rethrottle = false) => Throttler.Throttle(name, miliseconds, rethrottle);

    public static void Reset(string name) => Throttler.Reset(name);

    public static bool Check(string name) => Throttler.Check(name);

    public static long GetRemainingTime(string name, bool allowNegative = false) => Throttler.GetRemainingTime(name, allowNegative);

    public static void ImGuiPrintDebugInfo() => Throttler.ImGuiPrintDebugInfo();
}
