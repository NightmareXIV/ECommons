using Dalamud.Plugin;
using System.Runtime.Loader;

namespace ECommons.Reflection;
internal class CachedPluginEntry
{
    internal object Plugin;
    internal AssemblyLoadContext Context;

    internal CachedPluginEntry(object plugin, AssemblyLoadContext context)
    {
        Plugin = plugin;
        Context = context;
    }
}
