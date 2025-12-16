using System.Reflection;

namespace ECommons.EzIpcManager;
public enum SafeWrapper
{
    /// <summary>
    /// Directly call IPC, don't use any wrapper
    /// </summary>
    [Obfuscation] None,
    /// <summary>
    /// Only catch and discard IPCException
    /// </summary>
    [Obfuscation] IPCException,
    /// <summary>
    /// Catch and discard all exceptions
    /// </summary>
    [Obfuscation] AnyException,
    /// <summary>
    /// Inherit default setting from <see cref="EzIPC.Init"/> call
    /// </summary>
    [Obfuscation] Inherit = 255
}
