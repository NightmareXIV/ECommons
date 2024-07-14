using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Singletons;

/// <summary>
/// Sets service creation priority. Default is 0; the less is priority, the later will service be loaded, and earlier - disposed.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class PriorityAttribute : Attribute
{
    public readonly int Priority;

    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
