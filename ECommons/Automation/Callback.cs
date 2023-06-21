using Dalamud.Logging;
using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.Automation
{
    public unsafe static class Callback
    {
        internal delegate byte AtkUnitBase_FireCallbackDelegate(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState);
        internal static AtkUnitBase_FireCallbackDelegate FireCallback = null;

        public static readonly AtkValue ZeroAtkValue = new() { Type = 0, Int = 0 };

        internal static void Initialize()
        {
            var ptr = Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 8B 4C 24 20 0F B6 D8");
            FireCallback = Marshal.GetDelegateForFunctionPointer<AtkUnitBase_FireCallbackDelegate>(ptr);
            PluginLog.Information($"Initialized Callback module, FireCallback = 0x{ptr:X16}");
        }

        public static void FireRaw(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState = 0)
        {
            if (FireCallback == null) Initialize();
            FireCallback(Base, valueCount, values, updateState);
        }
        
        public static void Fire(AtkUnitBase* Base, bool updateState, params object[] values)
        {
            if (Base == null) throw new Exception("Null UnitBase");
            var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
            if (atkValues == null) return;
            try
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var v = values[i];
                    switch (v)
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
                List<string> CallbackValues = new();
                for(var i = 0; i < values.Length; i++)
                {
                    CallbackValues.Add($"    Value {i}: [input: {values[i]}/{values[i]?.GetType().Name}] -> {DecodeValue(atkValues[i])})");
                }
                PluginLog.Verbose($"Firing callback: {MemoryHelper.ReadStringNullTerminated((nint)Base->Name)}, valueCount = {values.Length}, updateStatte = {updateState}, values:\n");
                FireRaw(Base, values.Length, atkValues, (byte)(updateState ?1:0));
            }
            finally
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (atkValues[i].Type == ValueType.String)
                    {
                        Marshal.FreeHGlobal(new IntPtr(atkValues[i].String));
                    }
                }
                Marshal.FreeHGlobal(new IntPtr(atkValues));
            }
        }

        public static string DecodeValues(int cnt, AtkValue* values)
        {
            var atkValueList = new List<string>();
            try
            {
                for (var i = 0; i < cnt; i++)
                {
                    atkValueList.Add(DecodeValue(values[i]));
                }
            }
            catch (Exception e)
            {
                e.Log();
            }
            return atkValueList.Join("\n");
        }

        public static string DecodeValue(AtkValue a)
        {
            var str = new StringBuilder(a.Type.ToString()).Append(": ");
            switch (a.Type)
            {
                case ValueType.Int:
                    {
                        str.Append(a.Int);
                        break;
                    }
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
            FireCallback = null;
        }
    }
}
