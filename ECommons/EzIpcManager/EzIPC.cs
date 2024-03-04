using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using System.Reflection;
using System.Linq.Expressions;
using static Dalamud.Game.Command.CommandInfo;
using System.Windows.Input;

namespace ECommons.EzIpcManager;
public static class EzIPC
{
    static List<Action> Unregister = [];

    public static void Init(object instance, string? prefix = null)
    {
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instance.GetType().GetMethods(ReflectionHelper.AllFlags))
        {
            var attr = method.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
            if(attr != null)
            {
                var ipcName = attr.IPCName ?? method.Name;
                MethodInfo? reg = null;
                foreach(var m in Svc.PluginInterface.GetType().GetMethods(ReflectionHelper.AllFlags))
                {
                    if(m.Name == "GetIpcProvider" && m.IsGenericMethod && m.GetGenericArguments().Length == method.GetParameters().Length + 1)
                    {
                        reg = m;
                        break;
                    }
                }
                if(reg == null) throw new ArgumentNullException("Could not retrieve GetIpcProvider. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                var isAction = method.ReturnType == typeof(void);
                var genericArray = (Type[])[..method.GetParameters().Select(x => x.ParameterType), isAction ? typeof(object) : method.ReturnType];
                var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                var name = $"{prefix}.{ipcName}";
                PluginLog.Information($"[EzIPC] Registering IPC method {name} with method {instance.GetType().FullName}.{method.Name}");
                genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction?"RegisterAction":"RegisterFunc", [ReflectionHelper.CreateDelegate(method, instance)], true);
                Unregister.Add(() => 
                {
                    PluginLog.Information($"[EzIPC] Unregistering IPC method {name}");
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "UnregisterAction" : "UnregisterFunc", [], true);
                });
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
}
