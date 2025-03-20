using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    /// <summary>
    /// Returns <see cref="UInt32"/> representation of <see cref="Single"/>.
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static uint AsUInt32(this float f) => *(uint*)&f;

    /// <summary>
    /// Converts <see cref="UInt32"/> representation of <see cref="Single"/> into <see cref="Single"/>.
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public static float AsFloat(this uint u) => *(float*)&u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUint(this Vector4 color) => ImGui.ColorConvertFloat4ToU32(color);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(this uint color) => ImGui.ColorConvertU32ToFloat4(color);

    public static uint ToUInt(this ushort value) => value;
    public static uint ToUInt(this byte value) => value;
    public static uint ToUInt(this int value) => (uint)value;
    public static int ToInt(this byte value) => value;
    public static int ToInt(this ushort value) => value;
    public static int ToInt(this uint value) => (int)value;

    /// <summary>
    /// Attempts to parse integer
    /// </summary>
    /// <param name="number">Input string</param>
    /// <returns>Integer if parsing was successful, <see langword="null"/> if failed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? ParseInt(this string number)
    {
        if(int.TryParse(number, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// Attempts to parse byte array string separated by specified character.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static bool TryParseByteArray(string input, out byte[] output, char separator = ' ')
    {
        var str = input.Split(separator);
        output = new byte[str.Length];
        for(var i = 0; i < str.Length; i++)
        {
            if(!byte.TryParse(str[i], NumberStyles.HexNumber, null, out output[i]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Converts byte array to hex string where bytes are separated by a specified character
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToHexString(this IEnumerable<byte> bytes, char separator = ' ')
    {
        var first = true;
        var sb = new StringBuilder();
        foreach(var x in bytes)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                sb.Append(separator);
            }
            sb.Append($"{x:X2}");
        }
        return sb.ToString();
    }
}
