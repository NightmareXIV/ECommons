using System.Linq;
using System.Reflection;

namespace ECommons.Reflection;
#nullable disable

public static partial class ReflectionHelper
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    /// <summary>
    /// Gets field or property of an instance object.
    /// </summary>
    /// <param name="obj">Instance containing field/property.</param>
    /// <param name="name">Name of the field/property</param>
    /// <returns>Value of a field/property</returns>
    public static object GetFoP(this object obj, string name)
    {
        return obj.GetType().GetField(name, AllFlags)?.GetValue(obj) 
            ?? obj.GetType().GetProperty(name, AllFlags)?.GetValue(obj);
    }

    /// <summary>
    /// Gets field or property of an instance object.
    /// </summary>
    /// <param name="obj">Instance containing field/property.</param>
    /// <param name="name">Name of the field/property</param>
    /// <returns>Value of a field/property</returns>
    public static T GetFoP<T>(this object obj, string name)
    {
        return (T)GetFoP(obj, name);
    }

    /// <summary>
    /// Sets a field or property of an instance object.
    /// </summary>
    /// <param name="obj">Instance containing field/property.</param>
    /// <param name="name">Name of the field/property</param>
    /// <param name="value">Value to set</param>
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

    /// <summary>
    /// Attempts to call a non-generic instance method.
    /// </summary>
    /// <param name="obj">Instance containing method</param>
    /// <param name="name">Method's name</param>
    /// <param name="params">Method's parameters</param>
    /// <param name="matchExactArgumentTypes">Whether to search for exact method types. Set this to true if you're dealing with ambiguous overloads.</param>
    /// <returns>Object returned by the target method</returns>
    public static object Call(this object obj, string name, object[] @params, bool matchExactArgumentTypes = false)
    {
        MethodInfo info;
        if (!matchExactArgumentTypes)
        {
            info = obj.GetType().GetMethod(name, AllFlags);
        }
        else
        {
            info = obj.GetType().GetMethod(name, AllFlags, @params.Select(x => x.GetType()).ToArray());
        }
        return info.Invoke(obj, @params);
    }

    /// <summary>
    /// Attempts to call a non-generic instance method.
    /// </summary>
    /// <param name="obj">Instance containing method</param>
    /// <param name="name">Method's name</param>
    /// <param name="params">Method's parameters</param>
    /// <param name="matchExactArgumentTypes">Whether to search for exact method types. Set this to true if you're dealing with ambiguous overloads.</param>
    /// <returns>Object returned by the target method</returns>
    public static T Call<T>(this object obj, string name, object[] @params, bool matchExactArgumentTypes = false)
    {
        return (T)Call(obj, name, @params, matchExactArgumentTypes);
    }
} 
