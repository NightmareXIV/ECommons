using ECommons.DalamudServices;
using System;
using System.Collections.Generic;

namespace ECommons.EzEventManager;

/// <summary>
/// Provides wrapped access to Framework.Update event. Disposed automatically upon calling <see cref="ECommonsMain.Dispose"/>.
/// </summary>
public class EzFrameworkUpdate : IDisposable
{
    internal static List<EzFrameworkUpdate> Registered = [];
    internal Action Delegate;

    public EzFrameworkUpdate(Action @delegate)
    {
        Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
        Svc.Framework.Update += OnTrigger;
        Registered.Add(this);
    }

    internal void OnTrigger(object _) => Delegate();

    public void Dispose()
    {
        Svc.Framework.Update -= OnTrigger;
        Registered.Remove(this);
    }
}
