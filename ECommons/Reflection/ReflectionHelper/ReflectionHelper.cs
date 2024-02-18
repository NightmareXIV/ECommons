using ECommons.Logging;
using ECommons.DalamudServices;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace ECommons.Reflection;
#nullable disable

public static partial class ReflectionHelper
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

    public static object Call(this object obj, string name, object[] values, bool matchExactArgumentTypes = false)
    {
        MethodInfo info;
        if (!matchExactArgumentTypes)
        {
            info = obj.GetType().GetMethod(name, AllFlags);
        }
        else
        {
            info = obj.GetType().GetMethod(name, AllFlags, values.Select(x => x.GetType()).ToArray());
        }
        return info.Invoke(obj, values);
    }

    public static T Call<T>(this object obj, string name, object[] values, bool matchExactArgumentTypes = false)
    {
        return (T)Call(obj, name, values, matchExactArgumentTypes);
    }
} 
