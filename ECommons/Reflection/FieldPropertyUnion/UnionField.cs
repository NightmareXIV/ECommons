using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ECommons.Reflection.FieldPropertyUnion;
public class UnionField : IFieldPropertyUnion
{
    public readonly FieldInfo FieldInfo;
    public UnionField(FieldInfo fieldInfo)
    {
        FieldInfo = fieldInfo;
    }
    
    public string Name => FieldInfo.Name;

    public Type UnionType => FieldInfo.FieldType;

    public Type? DeclaringType => FieldInfo.DeclaringType;

    public MemberTypes MemberType => FieldInfo.MemberType;

    public Type? ReflectedType => FieldInfo.ReflectedType;

    public bool IsSpecialName => FieldInfo.IsSpecialName;

    public System.Reflection.Module Module => FieldInfo.Module;

    public IEnumerable<CustomAttributeData> CustomAttributes => FieldInfo.CustomAttributes;

    public bool IsCollectible => FieldInfo.IsCollectible;

    public object[] GetCustomAttributes(bool inherit) => FieldInfo.GetCustomAttributes(inherit);

    public object[] GetCustomAttributes(Type attributeType, bool inherit) => FieldInfo.GetCustomAttributes(attributeType, inherit);

    public object? GetRawConstantValue() => FieldInfo.GetRawConstantValue();

    public object? GetValue(object? obj) => FieldInfo.GetValue(obj);

    public bool IsDefined(Type attributeType, bool inherit) => FieldInfo.IsDefined(attributeType, inherit);

    public void SetValue(object? obj, object? value) => FieldInfo.SetValue(obj, value);

    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture) => FieldInfo.SetValue(obj, value, invokeAttr, binder, culture);
}
