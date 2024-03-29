using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzIpcManager;
public enum SafeWrapper
{
    /// <summary>
    /// Directly call IPC, don't use any wrapper
    /// </summary>
    None,
    /// <summary>
    /// Only catch IPCException
    /// </summary>
    IPCException,
    /// <summary>
    /// Catch all exceptions
    /// </summary>
    AnyException,
    /// <summary>
    /// Inherit default setting from <see cref="EzIPC.Init"/> call
    /// </summary>
    Inherit = 255
}
