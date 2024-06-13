using System;

namespace ECommons.EzIpcManager;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class EzIPCAttribute : Attribute
{
    public string? IPCName;
    public bool ApplyPrefix;
    public Type ActionLastGenericType;
    public SafeWrapper Wrapper = SafeWrapper.Inherit;

    /// <summary>
    /// Initializes <see cref="EzIPCAttribute"/>.
    /// </summary>
    /// <param name="iPCName">IPC method name.</param>
    /// <param name="applyPrefix">Whether to apply prefix before name or not.</param>
    /// <param name="actionLastGenericType">Dummy return type used as a last generic argument for actions. When omitted, typeof(object) is used.</param>
    /// <param name="wrapper">Wrapper type</param>
    public EzIPCAttribute(string? iPCName = null, bool applyPrefix = true, Type? actionLastGenericType = null, SafeWrapper wrapper = SafeWrapper.Inherit)
    {
        this.IPCName = iPCName;
        this.ApplyPrefix = applyPrefix;
        this.ActionLastGenericType = actionLastGenericType ?? typeof(object);
        this.Wrapper = wrapper;
    }
}
