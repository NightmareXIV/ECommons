using ECommons.Logging;
using ECommons.DalamudServices;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace ECommons.Reflection;
#nullable disable

public static class ReflectionHelper
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static object GetFoP(this object obj, string name)
    {
        return obj.GetType().GetField(name, AllFlags)?.GetValue(obj) 
            ?? obj.GetType().GetProperty(name, AllFlags)?.GetValue(obj);
    }

    public static T GetFoP<T>(this object obj, string name)
    {
        return (T)GetFoP(obj, name);
    }

    public static void SetFoP(this object obj, string name, object value)
    {
        var field = obj.GetType().GetField(name, AllFlags);
        if(field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            obj.GetType().GetProperty(name, AllFlags).SetValue(obj, value);
        }
    }

    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static object GetStaticFoP(this object obj, string type, string name)
    {
        return obj.GetType().Assembly.GetType(type).GetField(name, StaticFlags)?.GetValue(null)
            ?? obj.GetType().Assembly.GetType(type).GetProperty(name, StaticFlags)?.GetValue(null);
    }

    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static T GetStaticFoP<T>(this object obj, string type, string name)
    {
        return (T)GetStaticFoP(obj, type, name);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static void SetStaticFoP(this object obj, string type, string name, object value)
    {
        var field = obj.GetType().Assembly.GetType(type).GetField(name, StaticFlags);
        if (field != null)
        {
            field.SetValue(null, value);
        }
        else
        {
            obj.GetType().Assembly.GetType(type).GetProperty(name, StaticFlags).SetValue(null, value);
        }
    }

    public static object Call(this object obj, string name, params object[] values)
    {
        var info = obj.GetType().GetMethod(name, AllFlags, values.Select(x => x.GetType()).ToArray());
        return info.Invoke(obj, values);
    }

    public static T Call<T>(this object obj, string name, params object[] values)
    {
        return (T)Call(obj, name, values);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static object CallStatic(this object obj, string type, string name, params object[] values)
    {
        var info = obj.GetType().Assembly.GetType(type).GetMethod(name, AllFlags, values.Select(x => x.GetType()).ToArray());
        return info.Invoke(obj, values);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static T CallStatic<T>(this object obj, string type, string name, params object[] values) => (T)CallStatic(obj, type, name, values);



    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static object CallGeneric(this object obj, string methodName, System.Collections.Generic.IEnumerable<string> typeArguments, object[] values)
    {
        var genericArgs = typeArguments.Select(x => obj.GetType().Assembly.GetType(x, true)).ToArray();
        return obj.GetType().GetMethod(methodName, AllFlags, values.Select(x => x.GetType()).ToArray())
            .MakeGenericMethod(genericArgs)
            .Invoke(null, BindingFlags.Default, null, values, null);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static T CallGeneric<T>(this object obj, string methodName, IEnumerable<string> typeArguments, object[] values) => (T)CallGeneric(obj, methodName, typeArguments, values);

    public static object CallGeneric(this object obj, string methodName, System.Collections.Generic.IEnumerable<string> typeArguments, IEnumerable<Assembly> AssembliesToSearch, object[] values)
    {
        var genericArgs = new List<Type>();
        foreach(var x in typeArguments)
        {
            foreach(var a in AssembliesToSearch)
            {
                var type = a.GetType(x, false);
                if(type != null)
                {
                    genericArgs.Add(type);
                    break;
                }
            }
        }

        return obj.GetType().GetMethod(methodName, AllFlags, values.Select(x => x.GetType()).ToArray())
            .MakeGenericMethod([.. genericArgs])
            .Invoke(null, BindingFlags.Default, null, values, null);
    }

    public static object CallGenericStatic(IEnumerable<Assembly> AssembliesToSearch, string typeName, string methodName, IEnumerable<string> typeArguments, object[] values)
    {
        var valuesTypes = values.Select(x => x.GetType()).ToArray();
        MethodInfo methodInfo = null;
        foreach(var a in AssembliesToSearch)
        {
            var t = a.GetType(typeName, false);
            if (t != null)
            {
                methodInfo = t.GetMethod(methodName, AllFlags, valuesTypes);
                if (methodInfo != null) break;
            }
        }
        var genericArgs = new List<Type>();
        foreach (var x in typeArguments)
        {
            foreach (var a in AssembliesToSearch)
            {
                var type = a.GetType(x, false);
                if (type != null)
                {
                    genericArgs.Add(type);
                    break;
                }
            }
        }
        return methodInfo.MakeGenericMethod([.. genericArgs])
            .Invoke(null, BindingFlags.Default, null, values, null);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static object CallStaticOnGenericType(this object obj, string genericType, string methodName, System.Collections.Generic.IEnumerable<string> typeArguments, object[] values)
    {
        var genericArgs = typeArguments.Select(x => obj.GetType().Assembly.GetType(x, true)).ToArray();
        return obj.GetType().Assembly.
                GetType($"{genericType}`{genericArgs.Length}", true).MakeGenericType(genericArgs).
                GetMethod(methodName, AllFlags, values.Select(x => x.GetType()).ToArray()).Invoke(null, BindingFlags.Default, null, values, null);
    }


    /// <summary>
    /// Subject for future refactoring and changes!
    /// </summary>
    public static T CallStaticOnGenericType<T>(this object obj, string genericType, string methodName, System.Collections.Generic.IEnumerable<string> typeArguments, object[] values) => (T)CallStaticOnGenericType(obj, genericType, methodName, typeArguments, values);
} 
