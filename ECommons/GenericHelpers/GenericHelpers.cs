using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Memory;
using ECommons.ChatMethods;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ECommons.Interop;
using ECommons.Logging;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#nullable disable

namespace ECommons;

public static unsafe partial class GenericHelpers
{
    private static string UidPrefix = $"{Random.Shared.Next(0, 0xFFFF):X4}";
    private static ulong UidCnt = 0;
    public static string GetTemporaryId() => $"{UidPrefix}{UidCnt++:X}";

    public static bool TryGetValue<T>(this T? nullable, out T value) where T : struct
    {
        if(nullable.HasValue)
        {
            value = nullable.Value;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Generates range of numbers with step = 1.
    /// </summary>
    /// <param name="inclusiveStart"></param>
    /// <param name="inclusiveEnd"></param>
    /// <returns></returns>
    public static uint[] Range(uint inclusiveStart, uint inclusiveEnd)
    {
        var ret = new uint[inclusiveEnd - inclusiveStart + 1];
        for(var i = 0; i < ret.Length; i++)
        {
            ret[i] = (uint)(inclusiveStart + i);
        }
        return ret;
    }

    /// <summary>
    /// Generates range of numbers with step = 1.
    /// </summary>
    /// <param name="inclusiveStart"></param>
    /// <param name="inclusiveEnd"></param>
    /// <returns></returns>
    public static int[] Range(int inclusiveStart, int inclusiveEnd)
    {
        var ret = new int[inclusiveEnd - inclusiveStart + 1];
        for(var i = 0; i < ret.Length; i++)
        {
            ret[i] = (int)(inclusiveStart + i);
        }
        return ret;
    }

    public static bool AddressEquals(this IGameObject obj, IGameObject other)
    {
        return obj?.Address == other?.Address;
    }

    /// <summary>
    /// Retrieves entries from call stack in a form of single string. <b>Expensive.</b>
    /// </summary>
    /// <param name="maxFrames"></param>
    /// <returns></returns>
    public static string GetCallStackID(int maxFrames = 3)
    {
        try
        {
            if(maxFrames == 0)
            {
                maxFrames = int.MaxValue;
            }
            else
            {
                maxFrames--;
            }
            var stack = new StackTrace().GetFrames();
            if(stack.Length > 1)
            {
                return stack[1..Math.Min(stack.Length, maxFrames)].Select(x => x.GetMethod() == null ? "<unknown>" : $"{x.GetMethod().DeclaringType?.FullName}.{x.GetMethod().Name}").Join(" <- ");
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
        return "";
    }

#pragma warning disable
    /// <summary>
    /// Sets whether <see cref="User32.GetKeyState"/> or <see cref="User32.GetAsyncKeyState"/> will be used when calling <see cref="IsKeyPressed(Keys)"/> or <see cref="IsKeyPressed(LimitedKeys)"/>
    /// </summary>
#pragma warning restore
    public static bool UseAsyncKeyCheck = false;

    /// <summary>
    /// Checks if a key is pressed via winapi.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Whether the key is currently pressed</returns>
    public static bool IsKeyPressed(int key)
    {
        if(key == 0) return false;
        if(UseAsyncKeyCheck)
        {
            return Bitmask.IsBitSet(User32.GetKeyState(key), 15);
        }
        else
        {
            return Bitmask.IsBitSet(User32.GetAsyncKeyState(key), 15);
        }
    }

    /// <summary>
    /// Checks if a key is pressed via winapi.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Whether the key is currently pressed</returns>
    public static bool IsKeyPressed(LimitedKeys key) => IsKeyPressed((int)key);

    public static bool IsAnyKeyPressed(IEnumerable<LimitedKeys> keys) => keys.Any(IsKeyPressed);

    public static bool IsKeyPressed(IEnumerable<LimitedKeys> keys)
    {
        foreach(var x in keys)
        {
            if(IsKeyPressed(x)) return true;
        }
        return false;
    }

    public static bool IsKeyPressed(IEnumerable<int> keys)
    {
        foreach(var x in keys)
        {
            if(IsKeyPressed(x)) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if you are targeting object <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Whether you are targeting object <paramref name="obj"/>; <see langword="false"/> if <paramref name="obj"/> is <see langword="null"/></returns>
    public static bool IsTarget(this IGameObject obj)
    {
        return Svc.Targets.Target != null && obj != null && Svc.Targets.Target.Address == obj.Address;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOr<T>(this T source, Predicate<T> testFunction)
    {
        return source == null || testFunction(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] CreateArray<T>(this T o, uint num)
    {
        var arr = new T[num];
        for(var i = 0; i < arr.Length; i++)
        {
            arr[i] = o;
        }
        return arr;
    }

    /// <summary>
    /// Serializes and then deserializes object, returning result of deserialization using <see cref="Newtonsoft.Json"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns>Deserialized copy of <paramref name="obj"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T JSONClone<T>(this T obj)
    {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }

    public static void DeleteFileToRecycleBin(string path)
    {
        try
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        catch(Exception e)
        {
            e.LogWarning();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool If<T>(this T obj, Func<T, bool> func)
    {
        return func(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNull<T>(this T obj, [NotNullWhen(true)] out T outobj)
    {
        outobj = obj;
        return obj != null;
    }

    public static bool IsOccupied()
    {
        return Svc.Condition[ConditionFlag.Occupied]
               || Svc.Condition[ConditionFlag.Occupied30]
               || Svc.Condition[ConditionFlag.Occupied33]
               || Svc.Condition[ConditionFlag.Occupied38]
               || Svc.Condition[ConditionFlag.Occupied39]
               || Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
               || Svc.Condition[ConditionFlag.OccupiedInEvent]
               || Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
               || Svc.Condition[ConditionFlag.OccupiedSummoningBell]
               || Svc.Condition[ConditionFlag.WatchingCutscene]
               || Svc.Condition[ConditionFlag.WatchingCutscene78]
               || Svc.Condition[ConditionFlag.BetweenAreas]
               || Svc.Condition[ConditionFlag.BetweenAreas51]
               || Svc.Condition[ConditionFlag.InThatPosition]
               //|| Svc.Condition[ConditionFlag.TradeOpen]
               || Svc.Condition[ConditionFlag.Crafting]
               || Svc.Condition[ConditionFlag.Crafting40]
               || Svc.Condition[ConditionFlag.PreparingToCraft]
               || Svc.Condition[ConditionFlag.InThatPosition]
               || Svc.Condition[ConditionFlag.Unconscious]
               || Svc.Condition[ConditionFlag.MeldingMateria]
               || Svc.Condition[ConditionFlag.Gathering]
               || Svc.Condition[ConditionFlag.OperatingSiegeMachine]
               || Svc.Condition[ConditionFlag.CarryingItem]
               || Svc.Condition[ConditionFlag.CarryingObject]
               || Svc.Condition[ConditionFlag.BeingMoved]
               || Svc.Condition[ConditionFlag.Mounted2]
               || Svc.Condition[ConditionFlag.Mounting]
               || Svc.Condition[ConditionFlag.Mounting71]
               || Svc.Condition[ConditionFlag.ParticipatingInCustomMatch]
               || Svc.Condition[ConditionFlag.PlayingLordOfVerminion]
               || Svc.Condition[ConditionFlag.ChocoboRacing]
               || Svc.Condition[ConditionFlag.PlayingMiniGame]
               || Svc.Condition[ConditionFlag.Performing]
               || Svc.Condition[ConditionFlag.PreparingToCraft]
               || Svc.Condition[ConditionFlag.Fishing]
               || Svc.Condition[ConditionFlag.Transformed]
               || Svc.Condition[ConditionFlag.UsingHousingFunctions]
               || Svc.ClientState.LocalPlayer?.IsTargetable != true;
    }

    /// <summary>
    /// Attempts to parse player in a <see cref="SeString"/>. 
    /// </summary>
    /// <param name="sender"><see cref="SeString"/> from which to read player</param>
    /// <param name="senderStruct">Resulting player data</param>
    /// <returns>Whether operation succeeded</returns>
    public static bool TryDecodeSender(SeString sender, out Sender senderStruct)
    {
        if(sender == null)
        {
            senderStruct = default;
            return false;
        }
        foreach(var x in sender.Payloads)
        {
            if(x is PlayerPayload p)
            {
                senderStruct = new(p.PlayerName, p.World.RowId);
                return true;
            }
        }
        senderStruct = default;
        return false;
    }

    public static SeStringBuilder Add(this SeStringBuilder b, IEnumerable<Payload> payloads)
    {
        foreach(var x in payloads)
        {
            b = b.Add(x);
        }
        return b;
    }

    public static string GetTerritoryName(this Number terr)
    {
        var t = Svc.Data.GetExcelSheet<TerritoryType>().GetRowOrDefault(terr);
        return $"{terr} | {t?.ContentFinderCondition.ValueNullable?.Name.ToString().Default(t?.PlaceName.ValueNullable?.Name.ToString())}";
    }

    [Obsolete($"Please use ExcelWorldHelper.TryGetWorldByName")]
    public static bool TryGetWorldByName(string world, out Lumina.Excel.Sheets.World worldId) => ExcelWorldHelper.TryGetWorldByName(world, out worldId);

    public static Vector4 Invert(this Vector4 v)
    {
        return v with { X = 1f - v.X, Y = 1f - v.Y, Z = 1f - v.Z };
    }

    public static ref int ValidateRange(this ref int i, int min, int max)
    {
        if(i > max) i = max;
        if(i < min) i = min;
        return ref i;
    }

    public static ref float ValidateRange(this ref float i, float min, float max)
    {
        if(i > max) i = max;
        if(i < min) i = min;
        return ref i;
    }

    public static bool IsNoConditions()
    {
        if(!Svc.Condition[ConditionFlag.NormalConditions]) return false;
        for(var i = 2; i < 100; i++)
        {
            if(i == (int)ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance) continue;
            if(Svc.Condition[i]) return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Invert(this bool b, bool invert)
    {
        return invert ? !b : b;
    }

    public static void ShellStart(string s)
    {
        Safe(delegate
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = s,
                UseShellExecute = true
            });
        }, (e) =>
        {
            Notify.Error($"Could not open {s.Cut(60)}\n{e}");
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetParsedSeSetingColor(int percent)
    {
        if(percent < 25)
        {
            return 3;
        }
        else if(percent < 50)
        {
            return 45;
        }
        else if(percent < 75)
        {
            return 37;
        }
        else if(percent < 95)
        {
            return 541;
        }
        else if(percent < 99)
        {
            return 500;
        }
        else if(percent == 99)
        {
            return 561;
        }
        else if(percent == 100)
        {
            return 573;
        }
        else
        {
            return 518;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch(Exception e)
        {
            if(!suppressErrors) PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, Action<string, object[]> logAction)
    {
        try
        {
            a();
        }
        catch(Exception e)
        {
            logAction($"{e.Message}\n{e.StackTrace ?? ""}", Array.Empty<object>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, Action<string> fail, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch(Exception e)
        {
            try
            {
                fail(e.Message);
            }
            catch(Exception ex)
            {
                PluginLog.Error("Error while trying to process error handler:");
                PluginLog.Error($"{ex.Message}\n{ex.StackTrace ?? ""}");
                suppressErrors = false;
            }
            if(!suppressErrors) PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
        }
    }

    public static bool TryExecute(System.Action a)
    {
        try
        {
            a();
            return true;
        }
        catch(Exception e)
        {
            PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            return false;
        }
    }

    public static bool TryExecute<T>(Func<T> a, out T result)
    {
        try
        {
            result = a();
            return true;
        }
        catch(Exception e)
        {
            PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            result = default;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny<T>(this T obj, params T[] values)
    {
        return values.Any(x => x.Equals(obj));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny<T>(this T obj, IEnumerable<T> values)
    {
        return values.Any(x => x.Equals(obj));
    }

    public static void SetMinSize(this Window window, float width = 100, float height = 100) => SetMinSize(window, new Vector2(width, height));

    public static void SetMinSize(this Window window, Vector2 minSize)
    {
        window.SizeConstraints = new()
        {
            MinimumSize = minSize,
            MaximumSize = new Vector2(float.MaxValue)
        };
    }

    public static void SetSizeConstraints(this Window window, Vector2 minSize, Vector2 maxSize)
    {
        window.SizeConstraints = new()
        {
            MinimumSize = minSize,
            MaximumSize = maxSize
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete($"Use MemoryHelper.ReadSeString")]
    public static SeString ReadSeString(IntPtr memoryAddress, int maxLength) => GenericHelpers.ReadSeString(memoryAddress, maxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete($"Use MemoryHelper.ReadRaw")]
    public static void ReadRaw(IntPtr memoryAddress, int length, out byte[] value) => value = MemoryHelper.ReadRaw(memoryAddress, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete($"Use MemoryHelper.ReadRaw")]
    public static byte[] ReadRaw(IntPtr memoryAddress, int length) => MemoryHelper.ReadRaw(memoryAddress, length);
}
