using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Reflection;
#nullable disable
public static partial class ReflectionHelper
{
    /// <summary>
    /// Searches for a static method in the array of assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to search in</param>
    /// <param name="typeName">Fully qualified class name</param>
    /// <param name="typeArguments">Type arguments, if necessary. Leave as null if type is non-generic.</param>
    /// <param name="methodName">Static method name</param>
    /// <param name="methodTypeArguments">Method type arguments, if necessary. Leave as null if method is non-generic.</param>
    /// <param name="parameterTypes">Method parameters types.</param>
    /// <returns>MethodInfo of a method that was found or null.</returns>
    public static MethodInfo FindStaticMethodInAssemblies(IEnumerable<Assembly> assemblies, string typeName, Type[] typeArguments, string methodName, Type[] methodTypeArguments, Type[] parameterTypes)
    {
        MethodInfo methodInfo = null;
        foreach (var t in FindTypesInAssemblies(assemblies, [(typeName, typeArguments)]))
        {
            if (t != null)
            {
                methodInfo = t.GetMethod(methodName, AllFlags, parameterTypes);
                if (methodInfo != null)
                {
                    if(methodTypeArguments != null && methodTypeArguments.Length > 0)
                    {
                        try
                        {
                            methodInfo = methodInfo.MakeGenericMethod(methodTypeArguments);
                        }
                        catch (Exception) 
                        {
                            methodInfo = null;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return methodInfo;
    }

    /// <summary>
    /// Searches for specified types in specified assembly list.
    /// </summary>
    /// <param name="assemblies">Assemblies to search in</param>
    /// <param name="typeNames">A list of requested types names</param>
    /// <returns>A list of requested types. Please check resulting list length to ensure all types were found.</returns>
    public static List<Type> FindTypesInAssemblies(IEnumerable<Assembly> assemblies, IEnumerable<string> typeNames)
    {
        var genericArgs = new List<Type>();
        foreach (var x in typeNames)
        {
            foreach (var a in assemblies)
            {
                var type = a.GetType(x, false);
                if (type != null)
                {
                    genericArgs.Add(type);
                    break;
                }
            }
        }
        return genericArgs;
    }


    public static List<Type> FindTypesInAssemblies(IEnumerable<Assembly> assemblies, IEnumerable<(string TypeName, Type[] TypeArguments)> typeNames)
    {
        var genericArgs = new List<Type>();
        foreach (var x in typeNames)
        {
            foreach (var a in assemblies)
            {
                var type = a.GetType(x.TypeName, false);
                if (type != null)
                {
                    if(x.TypeArguments != null && x.TypeArguments.Length > 0)
                    {
                        try
                        {
                            type = type.MakeGenericType(x.TypeArguments);
                            genericArgs.Add(type);
                        }
                        catch (Exception)
                        {
                            type = null;
                        }
                    }
                    else
                    {
                        genericArgs.Add(type);
                    }
                    break;
                }
            }
        }
        return genericArgs;
    }

    /// <summary>
    /// Converts array of objects into array of these objects' types.
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    public static Type[] GetTypes(this IEnumerable<object> objects)
    {
        return objects.Select(x => x.GetType()).ToArray();
    }

    public static Delegate CreateDelegate(MethodInfo methodInfo, object target)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals((typeof(void)));
        var types = methodInfo.GetParameters().Select(p => p.ParameterType);

        if (isAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            types = types.Concat(new[] { methodInfo.ReturnType });
        }

        if (methodInfo.IsStatic)
        {
            return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        }

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
    }
}
