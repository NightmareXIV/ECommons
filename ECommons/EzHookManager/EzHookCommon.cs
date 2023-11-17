using Dalamud.Hooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzHookManager
{
    internal static class EzHookCommon
    {
        internal static List<IDalamudHook> RegisteredHooks = [];

        internal static void DisposeAll()
        {
            RegisteredHooks.ToArray().Each(x => x.Dispose());
            RegisteredHooks = null;
        }
    }
}
