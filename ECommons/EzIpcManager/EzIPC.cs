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
    /// <summary>
    /// Event that fires whenever an exception occurs in any of safe invocation wrapper methods.
    /// </summary>
    public static event Action<Exception>? OnSafeInvocationException;

    internal static void InvokeOnSafeInvocationException(Exception e) => OnSafeInvocationException?.Invoke(e);

    static List<EzIPCDisposalToken> Unregister = [];

    static Type[] FuncTypes = [typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>), typeof(Func<,,,,,>), typeof(Func<,,,,,,>), typeof(Func<,,,,,,,>), typeof(Func<,,,,,,,,>), typeof(Func<,,,,,,,,,>)];
    static Type[] ActionTypes = [typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>), typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>), typeof(Action<,,,,,,,,>), typeof(Action<,,,,,,,,,>)];

    /// <summary>
    /// Initializes IPC provider and subscriber for an instance type. Static methods or field/properties/properties will be ignored, register them separately via static Init if you must.<br></br>
    /// Each method that have <see cref="EzIPCAttribute"/> or <see cref="EzIPCEventAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.<br></br>
    /// Each Action and Function field/property that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Each Action field/property that have <see cref="EzIPCEventAttribute"/> will be assigned to become respective tag's event trigger. Make sure to explicitly specify prefix if you're interacting with other plugin's IPC.<br></br>
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="instance">Instance of a class that has EzIPC methods and field/properties/properties.</param>
    /// <param name="prefix">Name prefix</param>
    /// <param name="safeWrapper">Type of a safe invocation wrapper to be used for IPC calls. Wrappers, when used, will silently drop exceptions and return default object if invocation has failed. You can subscribe to <see cref="EzIPC.OnSafeInvocationException"/> event to observe these exceptions.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Array of disposal tokens that can be used to dispose registered providers and event subscription. <b>Typical use of EzIPC never has any need to store and deal with these tokens</b>; you only ever need them when you want to unregister IPC before your plugin's Dispose method is called.</returns>
    public static EzIPCDisposalToken[] Init(object instance, string? prefix = null, SafeWrapper safeWrapper = SafeWrapper.None) => Init(instance, instance.GetType(), prefix, safeWrapper);

    /// <summary>
    /// Initializes IPC provider and subscriber for a static type.<br></br>
    /// Each method that have <see cref="EzIPCAttribute"/> or <see cref="EzIPCEventAttribute"/> will be registered for IPC under "Prefix.IPCName" tag. If prefix is not specified, it is your plugin's internal name. If IPCName is not specified, it is method name.<br></br>
    /// Each Action and Function field/property that have <see cref="EzIPCAttribute"/> will be assigned delegate that represents respective GetIPCSubscriber. Each Action field/property that have <see cref="EzIPCEventAttribute"/> will be assigned to become respective tag's event trigger. Make sure to explicitly specify prefix if you're interacting with other plugin's IPC.<br></br>
    /// You do not need to dispose IPC methods in any way. Everything is disposed upon calling <see cref="ECommonsMain.Dispose"/>.
    /// </summary>
    /// <param name="staticType">Type of a static class that has EzIPC methods and field/properties/properties.</param>
    /// <param name="prefix">Name prefix</param>
    /// <param name="safeWrapper">Type of a safe invocation wrapper to be used for IPC calls. Wrappers, when used, will silently drop exceptions and return default object if invocation has failed. You can subscribe to <see cref="EzIPC.OnSafeInvocationException"/> event to observe these exceptions.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Array of disposal tokens that can be used to dispose registered providers and event subscription. <b>Typical use of EzIPC never has any need to store and deal with these tokens</b>; you only ever need them when you want to unregister IPC before your plugin's Dispose method is called.</returns>
    public static EzIPCDisposalToken[] Init(Type staticType, string? prefix = null, SafeWrapper safeWrapper = SafeWrapper.None) => Init(null, staticType, prefix, safeWrapper);

    private static EzIPCDisposalToken[] Init(object? instance, Type instanceType, string? prefix, SafeWrapper safeWrapper)
    {
        if (safeWrapper == SafeWrapper.Inherit) throw new InvalidOperationException($"{nameof(SafeWrapper.Inherit)} is only valid option when used in EzIPC attribute. Please choose your desired SafeWrapper.");
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
                    ipcName = ipcName.Replace("%m", method.Name);
                    ipcName = ipcName.Replace("%p", Svc.PluginInterface.InternalName);
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
                    ipcName = ipcName.Replace("%m", reference.Name);
                    ipcName = ipcName.Replace("%p", Svc.PluginInterface.InternalName);
                    var isNonGenericAction = reference.UnionType == typeof(Action);
                    if (isNonGenericAction || reference.UnionType.GetGenericTypeDefinition().EqualsAny([.. FuncTypes, .. ActionTypes]))
                    {
                        var wrapper = attr.Wrapper == SafeWrapper.Inherit ? safeWrapper : attr.Wrapper;
                        PluginLog.Information($"[EzIPC Subscriber] Attempting to assign IPC method to {instanceType.Name}.{reference.Name} with wrapper {wrapper}");
                        var isAction = isNonGenericAction || reference.UnionType.GetGenericTypeDefinition().EqualsAny(ActionTypes);
                        var genericArgsLen = reference.UnionType.GetGenericArguments().Length;
                        var reg = FindIpcSubscriber(genericArgsLen + (isAction ? 1 : 0)) ?? throw new NullReferenceException("Could not retrieve GetIpcSubscriber. Did you called EzIPC.Init before ECommonsMain.Init or specified more than 9 arguments?");
                        var genericArgs = reference.UnionType.IsGenericType ? reference.UnionType.GetGenericArguments() : [];
                        var adjustedGenericArgs = isAction ? [.. genericArgs, attr.ActionLastGenericType] : genericArgs;
                        var genericMethod = reg.MakeGenericMethod(adjustedGenericArgs);
                        var name = attr.ApplyPrefix ? $"{prefix}.{ipcName}" : ipcName;
                        var callerInfo = genericMethod.Invoke(Svc.PluginInterface, [name])!;
                        var invocationDelegate = ReflectionHelper.CreateDelegate(callerInfo.GetType().GetMethod(isAction ? "InvokeAction" : "InvokeFunc"), callerInfo);
                        if(wrapper != SafeWrapper.None)
                        {
                            var safeWrapperObj = CreateSafeWrapper(wrapper, adjustedGenericArgs) ?? throw new NullReferenceException("Safe wrapper creation failed. Please report this exception to developer.");
                            var safeWrapperMethod = safeWrapperObj.GetType().GetMethod(isAction ? "InvokeAction" : "InvokeFunction", ReflectionHelper.AllFlags);
                            safeWrapperObj.SetFoP(isAction ? "Action" : "Function", invocationDelegate);
                            reference.SetValue(instance, ReflectionHelper.CreateDelegate(safeWrapperMethod, safeWrapperObj));
                        }
                        else
                        {
                            reference.SetValue(instance, invocationDelegate);
                        }
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
                    ipcName = ipcName.Replace("%m", method.Name);
                    ipcName = ipcName.Replace("%p", Svc.PluginInterface.InternalName);
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
                    ipcName = ipcName.Replace("%m", reference.Name);
                    ipcName = ipcName.Replace("%p", Svc.PluginInterface.InternalName);
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
        OnSafeInvocationException = null;
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

    static object? CreateSafeWrapper(SafeWrapper wrapperKind, Type[] adjustedGenericArgs)
    {
        Type? type = null;
        if (wrapperKind == SafeWrapper.IPCException)
        {
            type = adjustedGenericArgs.Length switch
            {
                1 => typeof(SafeWrapperIPC.Wrapper<>),
                2 => typeof(SafeWrapperIPC.Wrapper<,>),
                3 => typeof(SafeWrapperIPC.Wrapper<,,>),
                4 => typeof(SafeWrapperIPC.Wrapper<,,,>),
                5 => typeof(SafeWrapperIPC.Wrapper<,,,,>),
                6 => typeof(SafeWrapperIPC.Wrapper<,,,,,>),
                7 => typeof(SafeWrapperIPC.Wrapper<,,,,,,>),
                8 => typeof(SafeWrapperIPC.Wrapper<,,,,,,,>),
                9 => typeof(SafeWrapperIPC.Wrapper<,,,,,,,,>),
                _ => throw new ArgumentOutOfRangeException(GetThrowString()),
            };
        }
        else
        {
            type = adjustedGenericArgs.Length switch
            {
                1 => typeof(SafeWrapperAny.Wrapper<>),
                2 => typeof(SafeWrapperAny.Wrapper<,>),
                3 => typeof(SafeWrapperAny.Wrapper<,,>),
                4 => typeof(SafeWrapperAny.Wrapper<,,,>),
                5 => typeof(SafeWrapperAny.Wrapper<,,,,>),
                6 => typeof(SafeWrapperAny.Wrapper<,,,,,>),
                7 => typeof(SafeWrapperAny.Wrapper<,,,,,,>),
                8 => typeof(SafeWrapperAny.Wrapper<,,,,,,,>),
                9 => typeof(SafeWrapperAny.Wrapper<,,,,,,,,>),
                _ => throw new ArgumentOutOfRangeException(GetThrowString()),
            };
        }
        type = type.MakeGenericType(adjustedGenericArgs);
        return Activator.CreateInstance(type);

        string GetThrowString() => $"Could not find safe wrapper of {wrapperKind} kind with {adjustedGenericArgs.Length} arguments";
    }
}
