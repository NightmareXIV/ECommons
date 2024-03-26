using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.EzIpcManager;

/// <summary>
/// Provides easier way to interact with Dalamud IPC.<br></br>
/// See EzIPC.md for example use.
/// </summary>
public static class EzIPC
{
    static List<EzIPCDisposalToken> Unregister = [];

    static Type[] FuncTypes = [typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>), typeof(Func<,,,,,>), typeof(Func<,,,,,,>), typeof(Func<,,,,,,,>), typeof(Func<,,,,,,,,>), typeof(Func<,,,,,,,,,>)];
    static Type[] ActionTypes = [typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>), typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>), typeof(Action<,,,,,,,,>), typeof(Action<,,,,,,,,,>)];

    /// <summary>
    /// Initializes IPC provider and subscriber for an instance type. Static methods or field/propertys/properties will be ignored, register them separately via static Init if you must.<br></br>
    /// Each method that have <see cref="EzIPCAttribute"/> or <see cref="EzIPCEventAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.<br></br>
    /// Each Action and Function field/property that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Each Action field/property that have <see cref="EzIPCEventAttribute"/> will be assigned to become respective tag's event trigger. Make sure to explicitly specify prefix if you're interacting with other plugin's IPC.<br></br>
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="instance">Instance of a class that has EzIPC methods and field/propertys/properties.</param>
    /// <param name="prefix">Name prefix</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Array of disposal tokens that can be used to dispose registered providers and event subscription. <b>Typical use of EzIPC never has any need to store and deal with these tokens</b>; you only ever need them when you want to unregister IPC before your plugin's Dispose method is called.</returns>
    public static EzIPCDisposalToken[] Init(object instance, string? prefix = null) => Init(instance, instance.GetType(), prefix);

    /// <summary>
    /// Initializes IPC provider and subscriber for a static type.<br></br>
    /// Each method that have <see cref="EzIPCAttribute"/> or <see cref="EzIPCEventAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.<br></br>
    /// Each Action and Function field/property that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Each Action field/property that have <see cref="EzIPCEventAttribute"/> will be assigned to become respective tag's event trigger. Make sure to explicitly specify prefix if you're interacting with other plugin's IPC.<br></br>
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="staticType">Type of a static class that has EzIPC methods and field/propertys/properties.</param>
    /// <param name="prefix">Name prefix</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Array of disposal tokens that can be used to dispose registered providers and event subscription. <b>Typical use of EzIPC never has any need to store and deal with these tokens</b>; you only ever need them when you want to unregister IPC before your plugin's Dispose method is called.</returns>
    public static EzIPCDisposalToken[] Init(Type staticType, string? prefix = null) => Init(null, staticType, prefix);

    private static EzIPCDisposalToken[] Init(object? instance, Type instanceType, string? prefix)
    {
        var ret = new List<EzIPCDisposalToken>();
        var bFlags = BindingFlags.Public | BindingFlags.NonPublic | (instance != null ? BindingFlags.Instance : BindingFlags.Static);
        //init provider
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instanceType.GetMethods(bFlags))
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
                    var genericArray = (Type[])[.. method.GetParameters().Select(x => x.ParameterType), isAction ? attr.ActionLastGenericType : method.ReturnType];
                    var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                    var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                    PluginLog.Information($"[EzIPC Provider] Registering IPC method {name} with method {instanceType.FullName}.{method.Name}");
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "RegisterAction" : "RegisterFunc", [ReflectionHelper.CreateDelegate(method, instance)], true);
                    var token = new EzIPCDisposalToken(name, false, () =>
                    {
                        PluginLog.Information($"[EzIPC Provider] Unregistering IPC method {name}");
                        genericMethod.Invoke(Svc.PluginInterface, [name]).Call(isAction ? "UnregisterAction" : "UnregisterFunc", [], true);
                    });
                    ret.Add(token);
                    Unregister.Add(token);
                }
            }
            catch(Exception e)
            {
                PluginLog.Error($"[EzIPC Provider] Failed to initialize provider for {instanceType.Name}.{method.Name}");
                e.Log();
            }
        }

        //init subscriber
        foreach (var reference in instanceType.GetFieldPropertyUnions(bFlags))
        {
            try
            {
                var attr = reference.GetCustomAttributes(true).OfType<EzIPCAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    var ipcName = attr.IPCName ?? reference.Name;
                    var isNonGenericAction = reference.UnionType == typeof(Action);
                    if (isNonGenericAction || reference.UnionType.GetGenericTypeDefinition().EqualsAny([.. FuncTypes, .. ActionTypes]))
                    {
                        PluginLog.Information($"[EzIPC Subscriber] Attempting to assign IPC method to {instanceType.Name}.{reference.Name}");
                        var isAction = isNonGenericAction || reference.UnionType.GetGenericTypeDefinition().EqualsAny(ActionTypes);
                        var reg = FindIpcSubscriber(reference.UnionType.GetGenericArguments().Length + (isAction ? 1 : 0)) ?? throw new NullReferenceException("Could not retrieve GetIpcSubscriber. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                        var genericArgs = reference.UnionType.IsGenericType ? reference.UnionType.GetGenericArguments() : [];
                        var genericMethod = reg.MakeGenericMethod(isAction ? [.. genericArgs, attr.ActionLastGenericType] : genericArgs);
                        var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                        var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                        reference.SetValue(instance, ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod(isAction ? "InvokeAction" : "InvokeFunc"), callerInfo));
                    }
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Subscriber] Failed to initialize subscriber for {instanceType.Name}.{reference.Name}");
                e.Log();
            }
        }

        //init subscriber event
        prefix ??= Svc.PluginInterface.InternalName;
        foreach (var method in instanceType.GetMethods(bFlags))
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
                    var genericArray = (Type[])[.. method.GetParameters().Select(x => x.ParameterType), attr.ActionLastGenericType];
                    var genericMethod = reg.MakeGenericMethod([.. genericArray]);
                    var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                    PluginLog.Information($"[EzIPC Subscriber] Registering IPC event {name} with method {instanceType.FullName}.{method.Name}");
                    var d = ReflectionHelper.CreateDelegate(method, instance);
                    genericMethod.Invoke(Svc.PluginInterface, [name]).Call("Subscribe", [d], true);
                    var token = new EzIPCDisposalToken(name, true, () =>
                    {
                        PluginLog.Information($"[EzIPC Subscriber] Unregistering IPC event {name}");
                        genericMethod.Invoke(Svc.PluginInterface, [name]).Call("Unsubscribe", [d], true);
                    });
                    Unregister.Add(token);
                    ret.Add(token);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Subscriber] Failed to subscribe for event for {instanceType.Name}.{method.Name}");
                e.Log();
            }
        }

        //init provider event
        foreach (var reference in instanceType.GetFieldPropertyUnions(bFlags))
        {
            try
            {
                var attr = reference.GetCustomAttributes(true).OfType<EzIPCEventAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    var ipcName = attr.IPCName ?? reference.Name;
                    var isNonGenericAction = reference.UnionType == typeof(Action);
                    if (isNonGenericAction || reference.UnionType.GetGenericTypeDefinition().EqualsAny(ActionTypes))
                    {
                        PluginLog.Information($"[EzIPC Provider] Attempting to assign IPC event to {instanceType.Name}.{reference.Name}");
                        var reg = FindIpcProvider(reference.UnionType.GetGenericArguments().Length + 1) ?? throw new NullReferenceException("Could not retrieve GetIpcProvider. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                        var genericArgs = reference.UnionType.IsGenericType ? reference.UnionType.GetGenericArguments() : [];
                        var genericMethod = reg.MakeGenericMethod([.. genericArgs, attr.ActionLastGenericType]);
                        var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                        var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                        reference.SetValue(instance, ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod("SendMessage"), callerInfo));
                    }
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"[EzIPC Provider] Failed to initialize event provider for {instanceType.Name}.{reference.Name}");
                e.Log();
            }
        }
        return [.. ret];
    }

    internal static void Dispose()
    {
        foreach(var token in Unregister)
        {
            try
            {
                token.Dispose();
            }
            catch(Exception e)
            {
                PluginLog.Error($"Error while unregistering IPC");
                e.Log();
            }
        }
        Unregister.Clear();
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
