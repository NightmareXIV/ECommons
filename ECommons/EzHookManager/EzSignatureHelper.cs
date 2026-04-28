using ECommons.Logging;
using ECommons.Reflection;
using InteropGenerator.Runtime;
using System;
using System.Reflection;
using TerraFX.Interop.Windows;

namespace ECommons.EzHookManager;
public static class EzSignatureHelper
{
    public static void Initialize(Object obj)
    {
        foreach(var x in obj.GetType().GetFields(ReflectionHelper.AllFlags))
        {
            var attr = x.GetCustomAttribute<EzHookAttribute>();
            var attr2 = x.GetCustomAttribute<EzHookFromCSAttribute>();
            try
            {
                if(attr != null)
                {
                    var detourName = attr.Detour ?? (x.Name.EndsWith("Hook") ? x.Name[..^4] : x.Name) + "Detour";
                    //PluginLog.Debug($"Detour: {detourName}");
                    var method = obj.GetType().GetMethod(detourName, ReflectionHelper.AllFlags);
                    var hook = Activator.CreateInstance(x.FieldType, attr.Signature, Delegate.CreateDelegate(x.FieldType.GenericTypeArguments[0], method!.IsStatic ? null : obj, method), attr.AutoEnable, attr.Offset);
                    x.SetValue(obj, hook);
                }
                else if(attr2 != null)
                {
                    var detourName = attr2.Detour ?? (x.Name.EndsWith("Hook") ? x.Name[..^4] : x.Name) + "Detour";
                    var method = obj.GetType().GetMethod(detourName, ReflectionHelper.AllFlags) ?? throw new InvalidOperationException($"Appropriate detour method could not be found for {x.Name}");
                    var genericType = x.FieldType.GenericTypeArguments.SafeSelect(0) ?? throw new InvalidOperationException($"{x.Name} has no generic argument.");
                    var delegatesClass = genericType?.DeclaringType ?? throw new InvalidOperationException($"{method.Name} has no declaring type (expected \"Delegates\" class)");
                    var parentStruct = delegatesClass.DeclaringType ?? throw new InvalidOperationException($"{delegatesClass.Name} has no declaring type (expected parent struct).");
                    var addressesClass = parentStruct.GetNestedType("Addresses", BindingFlags.Public | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"No \"Addresses\" nested type found in {parentStruct.Name}.");
                    var field = addressesClass.GetField(genericType.Name, ReflectionHelper.AllFlags) ?? throw new InvalidOperationException($"No field \"{genericType.Name}\" found in {parentStruct.Name}.Addresses.");

                    var addr = field.GetValue(null) as Address ?? throw new InvalidCastException($"Field '{field.Name}' is not of type Address or is null.");
                    var hook = Activator.CreateInstance(x.FieldType, addr.Value, Delegate.CreateDelegate(x.FieldType.GenericTypeArguments[0], method!.IsStatic ? null : obj, method), attr2.AutoEnable);
                }
            }
            catch(TargetInvocationException te)
            {
                te.Log();
                te.InnerException.Log();
            }
            catch(Exception e)
            {
                PluginLog.Error($"An error during initialization of attribute {attr?.ToString() ?? attr2?.ToString()} on {x.Name}");
                e.Log();
            }
        }
    }

    public static void Initialize(Type type)
    {
        foreach(var x in type.GetFields(ReflectionHelper.AllFlags))
        {
            var attr = x.GetCustomAttribute<EzHookAttribute>();
            var attr2 = x.GetCustomAttribute<EzHookFromCSAttribute>();
            try
            {
                if(attr != null)
                {
                    var detourName = attr.Detour ?? (x.Name.EndsWith("Hook") ? x.Name[..^4] : x.Name) + "Detour";
                    //PluginLog.Debug($"Detour: {detourName}");
                    var method = type.GetMethod(detourName, ReflectionHelper.AllFlags);
                    var hook = Activator.CreateInstance(x.FieldType, attr.Signature, Delegate.CreateDelegate(x.FieldType.GenericTypeArguments[0], null, method), attr.AutoEnable, attr.Offset);
                    x.SetValue(null, hook);
                }
                else if(attr2 != null)
                {
                    var detourName = attr2.Detour ?? (x.Name.EndsWith("Hook") ? x.Name[..^4] : x.Name) + "Detour";
                    var method = type.GetMethod(detourName, ReflectionHelper.AllFlags) ?? throw new InvalidOperationException($"Appropriate detour method could not be found for {x.Name}");
                    var genericType = x.FieldType.GenericTypeArguments.SafeSelect(0) ?? throw new InvalidOperationException($"{x.Name} has no generic argument.");
                    var delegatesClass = genericType?.DeclaringType ?? throw new InvalidOperationException($"{method.Name} has no declaring type (expected \"Delegates\" class)");
                    var parentStruct = delegatesClass.DeclaringType ?? throw new InvalidOperationException($"{delegatesClass.Name} has no declaring type (expected parent struct).");
                    var addressesClass = parentStruct.GetNestedType("Addresses", BindingFlags.Public | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"No \"Addresses\" nested type found in {parentStruct.Name}.");
                    var field = addressesClass.GetField(genericType.Name, ReflectionHelper.AllFlags) ?? throw new InvalidOperationException($"No field \"{genericType.Name}\" found in {parentStruct.Name}.Addresses.");

                    var addr = field.GetValue(null) as Address ?? throw new InvalidCastException($"Field '{field.Name}' is not of type Address or is null.");
                    var hook = Activator.CreateInstance(x.FieldType, addr.Value, Delegate.CreateDelegate(x.FieldType.GenericTypeArguments[0], null, method), attr2.AutoEnable);
                }
            }
            catch(TargetInvocationException te)
            {
                te.Log();
                te.InnerException.Log();
            }
            catch(Exception e)
            {
                PluginLog.Error($"An error during initialization of attribute {attr} on {x.Name}");
                e.Log();
            }
        }
    }

}
