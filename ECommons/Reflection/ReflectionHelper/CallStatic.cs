using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.Reflection;
#nullable disable
public static partial class ReflectionHelper
{
    /// <summary>
    /// Calls a static non-generic method.
    /// </summary>
    /// <param name="obj">Object on which assembly to search.</param>
    /// <param name="type">Fully qualified static class name</param>
    /// <param name="name">Method's name</param>
    /// <param name="values">Method's parameters</param>
    /// <returns></returns>
    public static object CallStatic(this object obj, string type, string name, object[] values)
    {
        var info = obj.GetType().Assembly.GetType(type).GetMethod(name, AllFlags, values.Select(x => x.GetType()).ToArray());
        return info.Invoke(obj, values);
    }

    /// <summary>
    /// Calls a static non-generic method.
    /// </summary>
    /// <param name="obj">Object on which assembly to search.</param>
    /// <param name="type">Fully qualified static class name</param>
    /// <param name="name">Method's name</param>
    /// <param name="values">Method's parameters</param>
    /// <returns></returns>
    public static T CallStatic<T>(this object obj, string type, string name, object[] values) => (T)CallStatic(obj, type, name, values);
    
    /// <summary>
    /// Ultimate static method calling method.
    /// </summary>
    /// <param name="assemblies">Assemblies to search method in.</param>
    /// <param name="typeName">Fully qualified type name.</param>
    /// <param name="typeArguments">Type arguments if generic, null otherwise</param>
    /// <param name="methodName">Static method name.</param>
    /// <param name="methodTypeArguments">Static method type arguments if generic, null otherwise.</param>
    /// <param name="parameters">Method parameters.</param>
    /// <returns></returns>
    public static object CallStatic(IEnumerable<Assembly> assemblies, string typeName, IEnumerable<string> typeArguments, string methodName, IEnumerable<string> methodTypeArguments, object[] parameters)
    {
        Type[] transformedTypeArguments = null;
        Type[] transformedMethodTypeArguments = null;
        if(typeArguments != null)
        {
            transformedTypeArguments = [.. FindTypesInAssemblies(assemblies, typeArguments)];
        }
        if(methodTypeArguments != null)
        {
            transformedMethodTypeArguments = [.. FindTypesInAssemblies(assemblies, methodTypeArguments)];
        }
        
        var method = FindStaticMethodInAssemblies(assemblies, typeName, transformedTypeArguments, methodName, transformedMethodTypeArguments, parameters.GetTypes());
        return method?.Invoke(null, parameters);
    }
}
