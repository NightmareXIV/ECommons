namespace ECommons.EzEventManager;
#nullable disable

internal static class EzEvent
{
    internal static void DisposeAll()
    {
        EzFrameworkUpdate.Registered.ToArray().Each(x => x.Dispose());
        EzFrameworkUpdate.Registered = null;
        EzLogout.Registered.ToArray().Each(x => x.Dispose());
        EzLogout.Registered = null;
        EzTerritoryChanged.Registered.ToArray().Each(x => x.Dispose());
        EzTerritoryChanged.Registered = null;
    }
}
