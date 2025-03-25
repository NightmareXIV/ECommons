using System;

namespace ECommons.EzHookManager;
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EzHookAttribute : Attribute
{
    public string Signature { get; private set; } = "";
    public int Offset { get; private set; } = 0;
    public string? Detour { get; private set; } = null;
    public bool AutoEnable { get; private set; } = true;

    public EzHookAttribute(string signature, bool autoEnable = true)
    {
        Setup(signature, 0, null, autoEnable);
    }

    public EzHookAttribute(string signature, int offset, bool autoEnable = true)
    {
        Setup(signature, offset, null, autoEnable);
    }

    public EzHookAttribute(string signature, string? detourName, bool autoEnable = true)
    {
        Setup(signature, 0, detourName, autoEnable);
    }

    public EzHookAttribute(string signature, int offset, string? detourName, bool autoEnable = true)
    {
        Setup(signature, offset, detourName, autoEnable);
    }

    private void Setup(string signature, int offset, string? detourName, bool autoEnable)
    {
        Signature = signature;
        Offset = offset;
        Detour = detourName;
        AutoEnable = autoEnable;
    }

    public override string? ToString()
    {
        return $"EzHook(Signature={Signature}, Offset={Offset}, Detour={Detour}, AutoEnable={AutoEnable})";
    }
}
