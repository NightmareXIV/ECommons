using System;

namespace ECommons.EzIpcManager;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public class EzIPCEventAttribute : Attribute
{
    public string? IPCName;
    public bool ApplyPrefix;
    public Type ActionLastGenericType;

    /// <summary>
    /// Initializes <see cref="EzIPCAttribute"/>.
    /// </summary>
    /// <param name="iPCName">IPC method name.</param>
    /// <param name="applyPrefix">Whether to apply prefix before name or not.</param>
    /// /// <param name="actionLastGenericType">Dummy return type used as a last generic argument for actions. When omitted, typeof(object) is used.</param>
    public EzIPCEventAttribute(string? iPCName = null, bool applyPrefix = true, Type? actionLastGenericType = null)
    {
        this.IPCName = iPCName;
        this.ApplyPrefix = applyPrefix;
        this.ActionLastGenericType = actionLastGenericType ?? typeof(object);
    }
}