using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.Reflection;
#nullable disable
public static partial class ReflectionHelper
{
    /// <summary>
    /// Calls a generic or non-generic method of an instance.
    /// </summary>
    /// <param name="instance">Instance containing method</param>
    /// <param name="methodName">Name of a method to call.</param>
    /// <param name="methodTypeArguments">Generic argument names. Pass empty array or null if a function is non-generic. Generic types only will be searched within <paramref name="instance"/> assembly.</param>
    /// <param name="parameters">Parameters to call method with.</param>
    /// <returns>Object returned by a method.</returns>
    public static object Call(this object instance, string methodName, IEnumerable<string> methodTypeArguments, object[] parameters) => Call(instance, [instance.GetType().Assembly], methodName, methodTypeArguments, parameters);

    /// <summary>
    /// Calls a generic or non-generic method of an instance.
    /// </summary>
    /// <param name="obj">Instance containing method</param>
    /// <param name="assemblies">Assemblies to search types in. Only required if <paramref name="methodTypeArguments"/> is used.</param>
    /// <param name="methodName">Name of a method to call.</param>
    /// <param name="methodTypeArguments">Generic argument names. Pass empty array or null if a function is non-generic, otherwise also populate <paramref name="assemblies"/> parameter.</param>
    /// <param name="parameters">Parameters to call method with.</param>
    /// <returns>Object returned by a method.</returns>
    public static object Call(this object obj, IEnumerable<Assembly>? assemblies, string methodName, IEnumerable<string>? methodTypeArguments, object[] parameters)
    {
        var methodInfo = obj.GetType().GetMethod(methodName, AllFlags, parameters.Select(x => x.GetType()).ToArray());
        if (methodInfo != null)
        {
            if (methodTypeArguments != null && methodTypeArguments.Any())
            {
                try
                {
                    methodInfo = methodInfo.MakeGenericMethod([.. FindTypesInAssemblies(assemblies, methodTypeArguments)]);
                }
                catch (Exception)
                {
                    methodInfo = null;
                }
            }
        }
        return methodInfo?.Invoke(obj, parameters);
    }

    /// <summary>
    /// Calls a generic or non-generic method of an instance.
    /// </summary>
    /// <param name="obj">Instance containing method</param>
    /// <param name="assemblies">Assemblies to search types in. Only required if <paramref name="methodTypeArguments"/> is used.</param>
    /// <param name="methodName">Name of a method to call.</param>
    /// <param name="methodTypeArguments">Generic argument names. Pass empty array or null if a function is non-generic, otherwise also populate <paramref name="assemblies"/> parameter.</param>
    /// <param name="parameters">Parameters to call method with.</param>
    /// <returns>Object returned by a method.</returns>
    public static T Call<T>(this object obj, IEnumerable<Assembly> assemblies, string methodName, IEnumerable<string> methodTypeArguments, object[] parameters) => (T)Call(obj, assemblies, methodName, methodTypeArguments, parameters);
}
