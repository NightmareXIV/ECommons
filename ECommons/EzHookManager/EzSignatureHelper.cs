using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Reflection;

namespace ECommons.EzHookManager;
public static class EzSignatureHelper
{
    public static void Initialize(Object obj)
    {
        foreach (var x in obj.GetType().GetFields(ReflectionHelper.AllFlags))
        {
            try
            {
                var attr = x.GetCustomAttribute<EzHookAttribute>();
                if (attr != null)
                {
                    var detourName = attr.Detour ?? (x.Name.EndsWith("Hook") ? x.Name[..^4] : x.Name) + "Detour";
                    //PluginLog.Debug($"Detour: {detourName}");
                    var method = obj.GetType().GetMethod(detourName, ReflectionHelper.AllFlags);
                    var hook = Activator.CreateInstance(x.FieldType, attr.Signature, Delegate.CreateDelegate(x.FieldType.GenericTypeArguments[0], method!.IsStatic?null:obj, method), attr.AutoEnable, attr.Offset);
                    x.SetValue(obj, hook);
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
    }

}
