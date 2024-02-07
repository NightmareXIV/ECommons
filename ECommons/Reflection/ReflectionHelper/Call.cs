using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Reflection;
#nullable disable
public static partial class ReflectionHelper
{
    public static object Call(this object obj, string methodName, IEnumerable<string> methodTypeArguments, object[] parameters) => Call(obj, [obj.GetType().Assembly], methodName, methodTypeArguments, parameters);

    public static object Call(this object obj, IEnumerable<Assembly> assemblies, string methodName, IEnumerable<string> methodTypeArguments, object[] parameters)
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
}
