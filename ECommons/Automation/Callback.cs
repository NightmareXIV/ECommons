using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;
#nullable disable

namespace ECommons.Automation;

public static unsafe class Callback
{
    private static Hook<AtkUnitBase.Delegates.FireCallback> AtkUnitBase_FireCallbackHook;

    public static readonly AtkValue ZeroAtkValue = new() { Type = 0, Int = 0 };

    public static void InstallHook()
    {
        AtkUnitBase_FireCallbackHook ??= Svc.Hook.HookFromAddress<AtkUnitBase.Delegates.FireCallback>(
            AtkUnitBase.MemberFunctionPointers.FireCallback,
            AtkUnitBase_FireCallbackDetour
        );

        if(AtkUnitBase_FireCallbackHook.IsEnabled)
        {
            PluginLog.Error("AtkUnitBase_FireCallbackHook is already enabled");
        }
        else
        {
            AtkUnitBase_FireCallbackHook.Enable();
            PluginLog.Information("AtkUnitBase_FireCallbackHook enabled");
        }
    }

    public static void UninstallHook()
    {
        if(!AtkUnitBase_FireCallbackHook.IsEnabled)
        {
            PluginLog.Error("AtkUnitBase_FireCallbackHook is already disabled");
        }
        else
        {
            AtkUnitBase_FireCallbackHook.Disable();
            PluginLog.Information("AtkUnitBase_FireCallbackHook disabled");
        }
    }

    private static bool AtkUnitBase_FireCallbackDetour(AtkUnitBase* Base, uint valueCount, AtkValue* values, bool updateState)
    {
        var ret = AtkUnitBase_FireCallbackHook?.Original(Base, valueCount, values, updateState);
        try
        {
            int valueCountInt = Convert.ToInt32(valueCount); // Could throw OverflowException since DecodeValues takes an int
            PluginLog.Debug($"Callback on {Base->Name.Read()}, valueCount={valueCount}, updateState={updateState}\n{DecodeValues(valueCountInt, values).Select(x => $"    {x}").Join("\n")}");
        }
        catch(Exception e)
        {
            e.Log();
        }
        return ret ?? false;
    }

    // Modified and kept for compatibility, but obsolete since you can just call FireCallback on the addon directly
    // could add obsolete attribute
    public static void FireRaw(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState = 0)
    {
        Base->FireCallback((uint)valueCount, values, updateState != 0);
    }

    public static void Fire(AtkUnitBase* Base, bool updateState, params object[] values)
    {
        if(Base == null) throw new Exception("Null UnitBase");
        var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
        if(atkValues == null) return;
        try
        {
            for(var i = 0; i < values.Length; i++)
            {
                var v = values[i];
                switch(v)
                {
                    case uint uintValue:
                        atkValues[i].Type = ValueType.UInt;
                        atkValues[i].UInt = uintValue;
                        break;
                    case int intValue:
                        atkValues[i].Type = ValueType.Int;
                        atkValues[i].Int = intValue;
                        break;
                    case float floatValue:
                        atkValues[i].Type = ValueType.Float;
                        atkValues[i].Float = floatValue;
                        break;
                    case bool boolValue:
                        atkValues[i].Type = ValueType.Bool;
                        atkValues[i].Byte = (byte)(boolValue ? 1 : 0);
                        break;
                    case string stringValue:
                        {
                            atkValues[i].Type = ValueType.String;
                            var stringBytes = Encoding.UTF8.GetBytes(stringValue);
                            var stringAlloc = Marshal.AllocHGlobal(stringBytes.Length + 1);
                            Marshal.Copy(stringBytes, 0, stringAlloc, stringBytes.Length);
                            Marshal.WriteByte(stringAlloc, stringBytes.Length, 0);
                            atkValues[i].String = (byte*)stringAlloc;
                            break;
                        }
                    case AtkValue rawValue:
                        {
                            atkValues[i] = rawValue;
                            break;
                        }
                    default:
                        throw new ArgumentException($"Unable to convert type {v.GetType()} to AtkValue");
                }
            }
            List<string> CallbackValues = [];
            for(var i = 0; i < values.Length; i++)
            {
                CallbackValues.Add($"    Value {i}: [input: {values[i]}/{values[i]?.GetType().Name}] -> {DecodeValue(atkValues[i])})");
            }
            PluginLog.Verbose($"Firing callback: {Base->Name.Read()}, valueCount = {values.Length}, updateStatte = {updateState}, values:\n");
            Base->FireCallback((uint)values.Length, atkValues, updateState);
        }
        finally
        {
            for(var i = 0; i < values.Length; i++)
            {
                if(atkValues[i].Type == ValueType.String)
                {
                    Marshal.FreeHGlobal(new IntPtr(atkValues[i].String));
                }
            }
            Marshal.FreeHGlobal(new IntPtr(atkValues));
        }
    }

    // Would need to convert cnt to uint but for compatibility kept as int
    // Only an issue in detour which will just throw OverflowException and get logged
    public static List<string> DecodeValues(int cnt, AtkValue* values)
    {
        var atkValueList = new List<string>();
        try
        {
            for(var i = 0; i < cnt; i++)
            {
                atkValueList.Add(DecodeValue(values[i]));
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
        return atkValueList;
    }

    public static string DecodeValue(AtkValue a)
    {
        var str = new StringBuilder(a.Type.ToString()).Append(": ");
        switch(a.Type)
        {
            case ValueType.Int:
                {
                    str.Append(a.Int);
                    break;
                }
            case ValueType.String8:
            case ValueType.WideString:
            case ValueType.ManagedString:
            case ValueType.String:
                {
                    str.Append(Marshal.PtrToStringUTF8(new IntPtr(a.String)));
                    break;
                }
            case ValueType.UInt:
                {
                    str.Append(a.UInt);
                    break;
                }
            case ValueType.Bool:
                {
                    str.Append(a.Byte != 0);
                    break;
                }
            default:
                {
                    str.Append($"Unknown Type: {a.Int}");
                    break;
                }
        }
        return str.ToString();
    }

    internal static void Dispose()
    {
        AtkUnitBase_FireCallbackHook?.Dispose();
        AtkUnitBase_FireCallbackHook = null;
    }
}
