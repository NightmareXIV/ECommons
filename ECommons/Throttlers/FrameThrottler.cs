using System.Collections.Generic;

namespace ECommons.Throttlers;

public static class FrameThrottler
{
    internal static FrameThrottler<string> Throttler = new();

    public static IReadOnlyCollection<string> ThrottleNames => Throttler.ThrottleNames;

    public static bool Throttle(string name, int frames = 60, bool rethrottle = false) => Throttler.Throttle(name, frames, rethrottle);

    public static bool Check(string name) => Throttler.Check(name);

    public static long GetRemainingTime(string name, bool allowNegative = false) => Throttler.GetRemainingTime(name, allowNegative);

    public static void ImGuiPrintDebugInfo() => Throttler.ImGuiPrintDebugInfo();
}
