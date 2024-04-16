using ECommons.Logging;
using System;

namespace ECommons.EzIpcManager;
/// <summary>
/// Represents EzIPC token, which can be used to manually dispose IPC when you want fine-grained control over disposing IPC. Any undisposed tokens are disposed during ECommonsMain's dispose so <b>you do not need to manually dispose tokens</b>.
/// </summary>
public sealed class EzIPCDisposalToken
{
    /// <summary>
    /// Full IPC tag
    /// </summary>
    public readonly string IpcTag;
    /// <summary>
    /// Whether the token is event subscription
    /// </summary>
    public readonly bool IsEvent;
    /// <summary>
    /// Whether the token was already disposed
    /// </summary>
    public bool IsDisposed { get; private set; } = false;
    readonly Action DisposeAction;

    internal EzIPCDisposalToken(string name, bool isEvent, Action disposeAction)
    {
        this.IpcTag = name ?? throw new ArgumentNullException(nameof(name));
        this.IsEvent = isEvent;
        this.DisposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
    }

    /// <summary>
    /// Disposes token, unregistering IPC provider or event subscription, if not already disposed.<br></br>
    /// <b>You do not need to call this method unless you specifically want to unregister your IPC before plugin unloads.</b>
    /// </summary>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            try
            {
                DisposeAction();
            }
            catch(Exception e)
            {
                PluginLog.Error($"[EzIPC Disposer] Error while disposing EzIPC");
                e.Log();
            }
        }
    }
}
