using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.EzIpcManager;
public static class EzIPC
{
    static List<Action> Unregister = [];

    static Type[] FuncTypes = [typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>), typeof(Func<,,,,,>), typeof(Func<,,,,,,>), typeof(Func<,,,,,,,>), typeof(Func<,,,,,,,,>), typeof(Func<,,,,,,,,,>)];
    static Type[] ActionTypes = [typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>), typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>), typeof(Action<,,,,,,,,>), typeof(Action<,,,,,,,,,>)];

    public static void Init(object instance, string? prefix = null)
    {
        //init provider
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instance.GetType().GetMethods(ReflectionHelper.AllFlags))
        {
            var attr = method.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
            if(attr != null)
            {
                PluginLog.Information($"[EzIPC Provider] Attempting to register {instance.GetType().Name}.{method.Name} as IPC ({method.GetParameters().Length})");
                var ipcName = attr.IPCName ?? method.Name;
                var reg = FindIpcProvider(method.GetParameters().Length + 1) ?? throw new ArgumentNullException("[EzIPC Provider] Could not retrieve GetIpcProvider. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                var isAction = method.ReturnType == typeof(void);
                var genericArray = (Type[])[..method.GetParameters().Select(x => x.ParameterType), isAction ? typeof(object) : method.ReturnType];
                var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                var name = $"{prefix}.{ipcName}";
                PluginLog.Information($"[EzIPC Provider] Registering IPC method {name} with method {instance.GetType().FullName}.{method.Name}");
                genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction?"RegisterAction":"RegisterFunc", [ReflectionHelper.CreateDelegate(method, instance)], true);
                Unregister.Add(() => 
                {
                    PluginLog.Information($"[EzIPC Provider] Unregistering IPC method {name}");
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "UnregisterAction" : "UnregisterFunc", [], true);
                });
            }
        }

        //init subscriber
        foreach (var field in instance.GetType().GetFields(ReflectionHelper.AllFlags))
        {
            var attr = field.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
            if (attr != null)
            {
                var ipcName = attr.IPCName ?? field.Name;
                var isNonGenericAction = field.FieldType == typeof(Action);
                if (isNonGenericAction || field.FieldType.GetGenericTypeDefinition().EqualsAny([.. FuncTypes, ..ActionTypes]))
                {
                    PluginLog.Information($"[EzIPC Subscriber] Attempting to assign IPC method to {instance.GetType().Name}.{field.Name}");
                    var isAction = isNonGenericAction || field.FieldType.GetGenericTypeDefinition().EqualsAny(ActionTypes);
                    var reg = FindIpcSubscriber(field.FieldType.GetGenericArguments().Length + (isAction?1:0)) ?? throw new ArgumentNullException("Could not retrieve GetIpcSubscriber. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                    var genericArgs = field.FieldType.IsGenericType ? field.FieldType.GetGenericArguments() : [];
                    var genericMethod = reg.MakeGenericMethod(isAction? [.. genericArgs, typeof(object)] : genericArgs);
                    var name = $"{prefix}.{ipcName}";
                    var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                    field.SetValue(instance, ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod(isAction?"InvokeAction":"InvokeFunc"), callerInfo));
                }
            }
        }
    }

    internal static void Dispose()
    {
        foreach(var method in Unregister)
        {
            try
            {
                method();
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
    }

    static MethodInfo? FindIpcProvider(int numGenericArgs)
    {
        foreach (var m in Svc.PluginInterface.GetType().GetMethods(ReflectionHelper.AllFlags))
        {
            if (m.Name == "GetIpcProvider" && m.IsGenericMethod && m.GetGenericArguments().Length == numGenericArgs)
            {
                return m;
            }
        }
        return null;
    }

    static MethodInfo? FindIpcSubscriber(int numGenericArgs)
    {
        foreach (var m in Svc.PluginInterface.GetType().GetMethods(ReflectionHelper.AllFlags))
        {
            if (m.Name == "GetIpcSubscriber" && m.IsGenericMethod && m.GetGenericArguments().Length == numGenericArgs)
            {
                return m;
            }
        }
        return null;
    }
}
