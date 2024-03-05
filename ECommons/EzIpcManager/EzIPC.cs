using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.EzIpcManager;

/// <summary>
/// Provides easier way to interact with Dalamud IPC.
/// See EzIPC.md for example use.
/// </summary>
public static class EzIPC
{
    static List<Action> Unregister = [];

    static Type[] FuncTypes = [typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>), typeof(Func<,,,,,>), typeof(Func<,,,,,,>), typeof(Func<,,,,,,,>), typeof(Func<,,,,,,,,>), typeof(Func<,,,,,,,,,>)];
    static Type[] ActionTypes = [typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>), typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>), typeof(Action<,,,,,,,,>), typeof(Action<,,,,,,,,,>)];

    /// <summary>
    /// Initializes IPC provider and subscriber.
    /// Each method that have <see cref="EzIPCAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.
    /// Each Action and Function field that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Make sure to explicitly specify prefix if you're calling other plugin's IPC.
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="instance">Instance of a class that has EzIPC methods and fields.</param>
    /// <param name="prefix">Name prefix</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Init(object instance, string? prefix = null) => Init(instance, instance.GetType(), prefix);

    /// <summary>
    /// Initializes IPC provider and subscriber.
    /// Each method that have <see cref="EzIPCAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.
    /// Each Action and Function field that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Make sure to explicitly specify prefix if you're calling other plugin's IPC.
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="staticType">Type of a static class that has EzIPC methods and fields.</param>
    /// <param name="prefix">Name prefix</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Init(Type staticType, string? prefix = null) => Init(null, staticType, prefix);

    /// <summary>
    /// Initializes IPC provider and subscriber.
    /// Each method that have <see cref="EzIPCAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.
    /// Each Action and Function field that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Make sure to explicitly specify prefix if you're calling other plugin's IPC.
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="instanceType">Type of a static class that has EzIPC methods and fields.</param>
    /// <param name="prefix">Name prefix</param>
    /// <param name="instance">Instance of a class that has EzIPC methods and fields.</param>
    /// <exception cref="ArgumentNullException"></exception>
    private static void Init(object? instance, Type instanceType, string? prefix)
    {
        //init provider
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instanceType.GetMethods(ReflectionHelper.AllFlags))
        {
            try
            {
                var attr = method.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    PluginLog.Information($"[EzIPC Provider] Attempting to register {instanceType.Name}.{method.Name} as IPC method ({method.GetParameters().Length})");
                    var ipcName = attr.IPCName ?? method.Name;
                    var reg = FindIpcProvider(method.GetParameters().Length + 1) ?? throw new NullReferenceException("[EzIPC Provider] Could not retrieve GetIpcProvider. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                    var isAction = method.ReturnType == typeof(void);
                    var genericArray = (Type[])[.. method.GetParameters().Select(x => x.ParameterType), isAction ? typeof(object) : method.ReturnType];
                    var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                    var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                    PluginLog.Information($"[EzIPC Provider] Registering IPC method {name} with method {instanceType.FullName}.{method.Name}");
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "RegisterAction" : "RegisterFunc", [ReflectionHelper.CreateDelegate(method, instance)], true);
                    Unregister.Add(() =>
                    {
                        PluginLog.Information($"[EzIPC Provider] Unregistering IPC method {name}");
                        genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "UnregisterAction" : "UnregisterFunc", [], true);
                    });
                }
            }
            catch(Exception e)
            {
                PluginLog.Error($"[EzIPC Provider] Failed to initialize provider for {instanceType.Name}.{method.Name}");
                e.Log();
            }
        }

        //init subscriber
        foreach (var field in instanceType.GetFields(ReflectionHelper.AllFlags))
        {
            try
            {
                var attr = field.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    var ipcName = attr.IPCName ?? field.Name;
                    var isNonGenericAction = field.FieldType == typeof(Action);
                    if (isNonGenericAction || field.FieldType.GetGenericTypeDefinition().EqualsAny([.. FuncTypes, .. ActionTypes]))
                    {
                        PluginLog.Information($"[EzIPC Subscriber] Attempting to assign IPC method to {instanceType.Name}.{field.Name}");
                        var isAction = isNonGenericAction || field.FieldType.GetGenericTypeDefinition().EqualsAny(ActionTypes);
                        var reg = FindIpcSubscriber(field.FieldType.GetGenericArguments().Length + (isAction ? 1 : 0)) ?? throw new NullReferenceException("Could not retrieve GetIpcSubscriber. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                        var genericArgs = field.FieldType.IsGenericType ? field.FieldType.GetGenericArguments() : [];
                        var genericMethod = reg.MakeGenericMethod(isAction ? [.. genericArgs, typeof(object)] : genericArgs);
                        var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                        var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                        field.SetValue(instance, ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod(isAction ? "InvokeAction" : "InvokeFunc"), callerInfo));
                    }
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Subscriber] Failed to initialize subscriber for {instanceType.Name}.{field.Name}");
                e.Log();
            }
        }

        //init subscriber event
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instanceType.GetMethods(ReflectionHelper.AllFlags))
        {
            try
            {
                var attr = method.GetCustomAttributes(true).OfType<EzIPCEventAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    PluginLog.Information($"[EzIPC Subscriber] Attempting to register {instanceType.Name}.{method.Name} as IPC event ({method.GetParameters().Length})");
                    var ipcName = attr.IPCName ?? method.Name;
                    var reg = FindIpcSubscriber(method.GetParameters().Length + 1) ?? throw new NullReferenceException("[EzIPC Provider] Could not retrieve FindIpcSubscriber. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                    if (method.ReturnType != typeof(void)) throw new InvalidOperationException($"Event method must have void return value");
                    var genericArray = (Type[])[.. method.GetParameters().Select(x => x.ParameterType), typeof(object)];
                    var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                    var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                    PluginLog.Information($"[EzIPC Subscriber] Registering IPC event {name} with method {instanceType.FullName}.{method.Name}");
                    var d = ReflectionHelper.CreateDelegate(method, instance);
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call("Subscribe", [d], true);
                    Unregister.Add(() =>
                    {
                        PluginLog.Information($"[EzIPC Subscriber] Unregistering IPC event {name}");
                        genericMethod.Invoke(Svc.PluginInterface, [name]).Call("Unsubscribe", [d], true);
                    });
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Subscriber] Failed to subscribe for event for {instanceType.Name}.{method.Name}");
                e.Log();
            }
        }

        //init provider event
        foreach (var field in instanceType.GetFields(ReflectionHelper.AllFlags))
        {
            try
            {
                var attr = field.GetCustomAttributes(true).OfType<EzIPCEventAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    var ipcName = attr.IPCName ?? field.Name;
                    var isNonGenericAction = field.FieldType == typeof(Action);
                    if (isNonGenericAction || field.FieldType.GetGenericTypeDefinition().EqualsAny(ActionTypes))
                    {
                        PluginLog.Information($"[EzIPC Provider] Attempting to assign IPC event to {instanceType.Name}.{field.Name}");
                        var reg = FindIpcProvider(field.FieldType.GetGenericArguments().Length + 1) ?? throw new NullReferenceException("Could not retrieve GetIpcProvider. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                        var genericArgs = field.FieldType.IsGenericType ? field.FieldType.GetGenericArguments() : [];
                        var genericMethod = reg.MakeGenericMethod([.. genericArgs, typeof(object)]);
                        var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                        var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                        field.SetValue(instance, ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod("SendMessage"), callerInfo));
                    }
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Provider] Failed to initialize event provider for {instanceType.Name}.{field.Name}");
                e.Log();
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
                PluginLog.Error($"Error while unregistering IPC");
                e.Log();
            }
        }
        Unregister = null!;
    }

    /// <summary>
    /// Searches for IPC provider function with specified number of generic arguments
    /// </summary>
    /// <param name="numGenericArgs"></param>
    /// <returns></returns>
    public static MethodInfo? FindIpcProvider(int numGenericArgs)
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

    /// <summary>
    /// Searches for IPC subscriber function with specified number of generic arguments
    /// </summary>
    /// <param name="numGenericArgs"></param>
    /// <returns></returns>
    public static MethodInfo? FindIpcSubscriber(int numGenericArgs)
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
