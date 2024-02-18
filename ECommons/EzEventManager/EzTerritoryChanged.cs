using ECommons.DalamudServices;
using System;
using System.Collections.Generic;

namespace ECommons.EzEventManager;

/// <summary>
/// Provides wrapped access to Framework.Update event. Disposed automatically upon calling <see cref="ECommonsMain.Dispose"/>.
/// </summary>
public class EzTerritoryChanged : IDisposable
{
    internal static List<EzTerritoryChanged> Registered = [];
    internal Action<ushort> Delegate;

    public EzTerritoryChanged(Action<ushort> @delegate)
    {
        Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
        Svc.ClientState.TerritoryChanged += Delegate;
        Registered.Add(this);
    }

    public void Dispose()
    {
        Svc.ClientState.TerritoryChanged -= Delegate;
        Registered.Remove(this);
    }
}
