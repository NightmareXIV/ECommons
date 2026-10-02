using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using TerraFX.Interop.Windows;

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

    public bool IsStatic => PropertyInfo.GetMethod?.IsStatic ?? PropertyInfo.SetMethod?.IsStatic ?? throw new InvalidOperationException("No GetMethod or SetMethod is available for property");

    public object[] GetCustomAttributes(bool inherit) => PropertyInfo.GetCustomAttributes(inherit);

    public object[] GetCustomAttributes(Type attributeType, bool inherit) => PropertyInfo.GetCustomAttributes(attributeType, inherit);
    public T? GetCustomAttribute<T>() where T : Attribute => PropertyInfo.GetCustomAttribute<T>();
    public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute => PropertyInfo.GetCustomAttributes<T>();

    public object? GetRawConstantValue() => PropertyInfo.GetRawConstantValue();

    public object? GetValue(object? obj) => PropertyInfo.GetValue(obj);

    public bool IsDefined(Type attributeType, bool inherit) => PropertyInfo.IsDefined(attributeType, inherit);

    public void SetValue(object? obj, object? value)
    {
        var setMethod = ReflectionHelper.GetAccessibleSetMethod(PropertyInfo) ?? throw new InvalidOperationException(
        $"No accessible setter found for {PropertyInfo.DeclaringType}.{PropertyInfo.Name}");
        setMethod.Invoke(obj,  [value]);
    }

    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture)
    {
        var setMethod = ReflectionHelper.GetAccessibleSetMethod(PropertyInfo) ?? throw new InvalidOperationException(
        $"No accessible setter found for {PropertyInfo.DeclaringType}.{PropertyInfo.Name}");
        setMethod.Invoke(obj, invokeAttr, binder, [value], culture);
    }
}
