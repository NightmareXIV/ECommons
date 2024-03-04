using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzIpcManager;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class EzIPCAttribute : Attribute
{
    public string? IPCName;

    public EzIPCAttribute(string? iPCName = null)
    {
        this.IPCName = iPCName;
    }
}
