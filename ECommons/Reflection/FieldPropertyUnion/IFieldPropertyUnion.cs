using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ECommons.Reflection.FieldPropertyUnion;
/// <summary>
/// Interface that represents union of FieldInfo and PropertyInfo. Allows access to common methods and properties.
/// </summary>
public interface IFieldPropertyUnion
{
    public Type UnionType { get; }
    public bool IsSpecialName { get; }
    public object? GetRawConstantValue();
    public object? GetValue(object? obj);
    public void SetValue(object? obj, object? value);
    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture);

    //MemberInfo
    public string Name { get; }
    public MemberTypes MemberType { get; }
    public Type? DeclaringType { get; }
    public Type? ReflectedType { get; }
    public System.Reflection.Module Module { get; }
    public bool IsDefined(Type attributeType, bool inherit);
    public object[] GetCustomAttributes(bool inherit);
    public object[] GetCustomAttributes(Type attributeType, bool inherit);
    public IEnumerable<CustomAttributeData> CustomAttributes { get; }
    public bool IsCollectible { get; }
}
