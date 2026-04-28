using System;
using System.Collections.Generic;
using System.Text;
using TerraFX.Interop.Windows;

namespace ECommons.EzHookManager;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EzHookFromCSAttribute : Attribute
{
    public string? Detour { get; private set; } = null;
    public bool AutoEnable { get; private set; } = true;

    public EzHookFromCSAttribute(string? detourName, bool autoEnable = true)
    {
        Setup(detourName, autoEnable);
    }

    public EzHookFromCSAttribute(bool autoEnable = true)
    {
        Setup(null, autoEnable);
    }

    private void Setup(string? detourName, bool autoEnable)
    {
        Detour = detourName;
        AutoEnable = autoEnable;
    }
}
