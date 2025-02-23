using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace ECommons.Singletons;
/// <summary>
/// Simple singleton service manager. Create a static class and fill it up with fields/properties of your singleton services, then call <see cref="Initialize"/> with your static class type whenever you want. Any uninitialized instances will be initialized upon <see cref="Initialize"/> call, and anything that implements <see cref="IDisposable"/> will be disposed together with <see cref="ECommonsMain.Dispose"/> call.
/// </summary>
public static class SingletonServiceManager
{
    internal static List<Type> Types = [];

    internal static void DisposeAll()
    {
        foreach(var x in Types)
        {
            foreach(var t in Enumerable.Reverse(x.GetFieldPropertyUnions(ReflectionHelper.AllFlags)))
            {
                var value = t.GetValue(null);
                var prio = t.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0;

                if(value is IDisposable disposable)
                {
                    try
                    {
                        PluginLog.Debug($"Disposing singleton instance of {t.UnionType.FullName}, priority={prio}");
                        disposable.Dispose();
                    }
                    catch(TargetInvocationException tie)
                    {
                        tie.Log();
                        tie.InnerException.Log();
                    }
                    catch(Exception e)
                    {
                        e.Log();
                    }
                }
                t.SetValue(null, null);

            }
        }
        Types = null!;
    }

    public static void Initialize(Type staticType)
    {
        Types.Add(staticType);
        foreach(var x in staticType.GetFieldPropertyUnions(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            var value = x.GetValue(null);
            if(value == null)
            {
                try
                {
                    var ctors = x.UnionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if(ctors.Length > 0)
                    {
                        var parameters = ctors[0].GetParameters();
                        var args = new object[parameters.Length];
                        for(var i = 0; i < parameters.Length; i++)
                        {
                            args[i] = parameters[i].ParameterType.IsValueType ? Activator.CreateInstance(parameters[i].ParameterType) : null;
                        }
                        PluginLog.Debug($"Creating singleton instance of {x.UnionType.FullName}");
                        x.SetValue(null, Activator.CreateInstance(x.UnionType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, args, null, null));
                    }
                    else
                        PluginLog.Warning($"Failed to create singleton instance of {x.UnionType.FullName}. Type does not have any constructors.");
                }
                catch(TargetInvocationException tie)
                {
                    tie.Log();
                    tie.InnerException.Log();
                }
                catch(Exception e)
                {
                    e.Log();
                }
            }
        }
    }
}
