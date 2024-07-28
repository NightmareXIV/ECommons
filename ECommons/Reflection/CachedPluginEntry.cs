using Dalamud.Plugin;
using System.Runtime.Loader;

namespace ECommons.Reflection;
internal class CachedPluginEntry
{
    internal IDalamudPlugin Plugin;
    internal AssemblyLoadContext Context;

    internal CachedPluginEntry(IDalamudPlugin plugin, AssemblyLoadContext context)
    {
        Plugin = plugin;
        Context = context;
    }
}
