using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Reflection;
internal class CachedPluginEntry
{
    internal IDalamudPlugin Plugin;
    internal AssemblyLoadContext Context;

    internal CachedPluginEntry(IDalamudPlugin plugin, AssemblyLoadContext context)
    {
        this.Plugin = plugin;
        this.Context = context;
    }
}
