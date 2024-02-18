using Dalamud.Hooking;
using System.Collections.Generic;

namespace ECommons.EzHookManager;

public static class EzHookCommon
{
    internal static List<IDalamudHook> RegisteredHooks = [];

    internal static void DisposeAll()
    {
        RegisteredHooks.ToArray().Each(x => x.Dispose());
        RegisteredHooks = null!;
    }

    public static int TrackMemory = 0;
}
