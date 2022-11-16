using System.Reflection;

namespace ECommons.Reflection;

public static class ReflectionHelper
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

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

    public static object GetStaticFoP(this object obj, string type, string name)
    {
        return obj.GetType().Assembly.GetType(type).GetField(name, AllFlags)?.GetValue(null)
            ?? obj.GetType().Assembly.GetType(type).GetProperty(name, AllFlags)?.GetValue(null);
    }

    public static T GetStaticFoP<T>(this object obj, string type, string name)
    {
        return (T)GetStaticFoP(obj, type, name);
    }

    public static void SetStaticFoP(this object obj, string type, string name, object value)
    {
        var field = obj.GetType().Assembly.GetType(type).GetField(name, AllFlags);
        if (field != null)
        {
            field.SetValue(null, value);
        }
        else
        {
            obj.GetType().Assembly.GetType(type).GetProperty(name, AllFlags).SetValue(null, value);
        }
    }
}
