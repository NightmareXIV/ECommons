using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzIpcManager;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class EzIPCEventAttribute : Attribute
{
    public string? IPCName;
    public bool ApplyPrefix;

    /// <summary>
    /// Initializes <see cref="EzIPCEventAttribute"/>.
    /// </summary>
    /// <param name="iPCName">IPC method name.</param>
    /// <param name="applyPrefix">Whether to apply prefix before name or not.</param>
    public EzIPCEventAttribute(string? iPCName = null, bool applyPrefix = true)
    {
        this.IPCName = iPCName;
        this.ApplyPrefix = applyPrefix;
    }
}