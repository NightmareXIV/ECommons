using System.Reflection;

namespace ECommons.EzIpcManager;
[Obfuscation(Exclude = true)]
public enum SafeWrapper
{
    /// <summary>
    /// Directly call IPC, don't use any wrapper
    /// </summary>
    None,
    /// <summary>
    /// Only catch and discard IPCException
    /// </summary>
    IPCException,
    /// <summary>
    /// Catch and discard all exceptions
    /// </summary>
    AnyException,
#pragma warning disable
    /// <summary>
    /// Inherit default setting from <see cref="EzIPC.Init"/> call
    /// </summary>
#pragma warning restore
    Inherit = 255
}
