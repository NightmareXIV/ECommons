using Dalamud.Plugin;
using System.Runtime.Loader;

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
