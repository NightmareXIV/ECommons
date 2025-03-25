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
    Type UnionType { get; }
    bool IsSpecialName { get; }
    object? GetRawConstantValue();
    object? GetValue(object? obj);
    void SetValue(object? obj, object? value);
    void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture);

    //MemberInfo
    string Name { get; }
    MemberTypes MemberType { get; }
    Type? DeclaringType { get; }
    Type? ReflectedType { get; }
    System.Reflection.Module Module { get; }
    bool IsDefined(Type attributeType, bool inherit);
    object[] GetCustomAttributes(bool inherit);
    object[] GetCustomAttributes(Type attributeType, bool inherit);
    T? GetCustomAttribute<T>() where T : Attribute;
    IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;

    IEnumerable<CustomAttributeData> CustomAttributes { get; }
    bool IsCollectible { get; }
}
