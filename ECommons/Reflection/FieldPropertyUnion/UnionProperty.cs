using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ECommons.Reflection.FieldPropertyUnion;
public class UnionProperty : IFieldPropertyUnion
{
    public readonly PropertyInfo PropertyInfo;
    public UnionProperty(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }

    public string Name => PropertyInfo.Name;

    public Type UnionType => PropertyInfo.PropertyType;

    public Type? DeclaringType => PropertyInfo.DeclaringType;

    public MemberTypes MemberType => PropertyInfo.MemberType;

    public Type? ReflectedType => PropertyInfo.ReflectedType;

    public bool IsSpecialName => PropertyInfo.IsSpecialName;

    public System.Reflection.Module Module => PropertyInfo.Module;

    public IEnumerable<CustomAttributeData> CustomAttributes => PropertyInfo.CustomAttributes;

    public bool IsCollectible => PropertyInfo.IsCollectible;

    public object[] GetCustomAttributes(bool inherit) => PropertyInfo.GetCustomAttributes(inherit);

    public object[] GetCustomAttributes(Type attributeType, bool inherit) => PropertyInfo.GetCustomAttributes(attributeType, inherit);

    public object? GetRawConstantValue() => PropertyInfo.GetRawConstantValue();

    public object? GetValue(object? obj) => PropertyInfo.GetValue(obj);

    public bool IsDefined(Type attributeType, bool inherit) => PropertyInfo.IsDefined(attributeType, inherit);

    public void SetValue(object? obj, object? value) => PropertyInfo.SetValue(obj, value);

    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture) => PropertyInfo.SetValue(obj, value, invokeAttr, binder, null, culture);
}
