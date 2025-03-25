using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.System.String;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemoveOtherChars(this string s, string charsToKeep)
        => new(s.ToArray().Where(charsToKeep.Contains).ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceByChar(this string s, string replaceWhat, string replaceWith, bool replaceWithWhole = false)
    {
        if(replaceWhat.Length != replaceWith.Length && !replaceWithWhole)
        {
            PluginLog.Error($"{nameof(replaceWhat)} and {nameof(replaceWith)} must be same length");
            return s;
        }
        for(var i = 0; i < replaceWhat.Length; i++)
        {
            if(replaceWithWhole)
            {
                s = s.Replace(replaceWhat[i].ToString(), replaceWith);
            }
            else
            {
                s = s.Replace(replaceWhat[i], replaceWith[i]);
            }
        }
        return s;
    }

    public static string ReplaceFirst(this string text, string search, string replace)
    {
        var pos = text.IndexOf(search);
        if(pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    /// <summary>
    /// Removes whitespaces, line breaks, tabs, etc from string.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Cleanup(this string s)
    {
        StringBuilder sb = new(s.Length);
        foreach(var c in s)
        {
            if(c == ' ' || c == '\n' || c == '\r' || c == '\t') continue;
            sb.Append(c);
        }
        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> Split(this string str, int chunkSize)
        => Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Default(this string s, string defaultValue)
    {
        if(string.IsNullOrEmpty(s)) return defaultValue;
        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Cut(this string s, int num)
    {
        if(s.Length <= num) return s;
        return s[0..num] + "...";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this string s, int num)
    {
        StringBuilder str = new();
        for(var i = 0; i < num; i++)
        {
            str.Append(s);
        }
        return str.ToString();
    }

    #region Comparisons
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithAny(this string source, params string[] values)
        => source.StartsWithAny(values, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithAny(this string source, StringComparison stringComparison = StringComparison.Ordinal, params string[] values)
        => source.StartsWithAny(values, stringComparison);

    public static bool StartsWithAny(this string source, IEnumerable<string> compareTo, StringComparison stringComparison = StringComparison.Ordinal)
    {
        foreach(var x in compareTo)
        {
            if(source.StartsWith(x, stringComparison)) return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny(this string obj, IEnumerable<string> values)
    {
        foreach(var x in values)
        {
            if(obj.Contains(x))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny(this string obj, params string[] values)
    {
        foreach(var x in values)
        {
            if(obj.Contains(x))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny(this string obj, StringComparison comp, params string[] values)
    {
        foreach(var x in values)
        {
            if(obj.Contains(x, comp))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(this string s, string other) => s.Equals(other, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCaseAny(this string obj, params string[] values) => values.Any(x => x.Equals(obj, StringComparison.OrdinalIgnoreCase));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCaseAny(this string obj, IEnumerable<string> values) => values.Any(x => x.Equals(obj, StringComparison.OrdinalIgnoreCase));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string s) => s == string.Empty ? null : s;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

    /// <summary>
    /// Returns <paramref name="s"/> when <paramref name="b"/> is <see langword="true"/>, <see langword="null"/> otherwise
    /// </summary>
    /// <param name="s"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string? NullWhenFalse(this string s, bool b) => b ? s : null;
    #endregion

    #region SeString/UTF8String
    [Obsolete("Dalamud has added their own ExtractText method for Lumina strings that is not compatible with ECommons. Therefore, extension method can not be used on Lumina strings anymore. For the consistency, ExtractText method is renamed to GetText.")]
    public static string ExtractText(this ReadOnlySeString s, bool onlyFirst = false) => s.GetText(onlyFirst);
    /// <summary>
    /// Discards any non-text payloads from <see cref="SeString"/>
    /// </summary>
    /// <param name="s"></param>
    /// <param name="onlyFirst">Whether to find first text payload and only return it</param>
    /// <returns>String that only includes text payloads</returns>
    public static string GetText(this ReadOnlySeString s, bool onlyFirst = false) => s.ToDalamudString().GetText(onlyFirst);

    [Obsolete("Dalamud has added their own ExtractText method for Lumina strings that is not compatible with ECommons. Therefore, extension method can not be used on Lumina strings anymore. For the consistency, ExtractText method is renamed to GetText.")]
    public static string ExtractText(this Utf8String s, bool onlyFirst = false) => s.GetText(onlyFirst);
    /// <summary>
    /// Reads SeString from unmanaged memory and discards any non-text payloads from <see cref="SeString"/>
    /// </summary>
    /// <param name="s"></param>
    /// <param name="onlyFirst">Whether to find first text payload and only return it</param>
    /// <returns>String that only includes text payloads</returns>
    public static string GetText(this Utf8String s, bool onlyFirst = false)
    {
        var str = GenericHelpers.ReadSeString(&s);
        return str.GetText(false);
    }

    [Obsolete("Dalamud has added their own ExtractText method for Lumina strings that is not compatible with ECommons. Therefore, extension method can not be used on Lumina strings anymore. For the consistency, ExtractText method is renamed to GetText.")]
    public static string ExtractText(this SeString seStr, bool onlyFirst = false) => seStr.GetText(onlyFirst);
    /// <summary>
    /// Discards any non-text payloads from <see cref="SeString"/>
    /// </summary>
    /// <param name="seStr"></param>
    /// <param name="onlyFirst">Whether to find first text payload and only return it</param>
    /// <returns>String that only includes text payloads</returns>
    public static string GetText(this SeString seStr, bool onlyFirst = false)
    {
        StringBuilder sb = new();
        foreach(var x in seStr.Payloads)
        {
            if(x is TextPayload tp)
            {
                sb.Append(tp.Text);
                if(onlyFirst) break;
            }
            if(x.Type == PayloadType.Unknown && x.Encode().SequenceEqual<byte>([0x02, 0x1d, 0x01, 0x03]))
            {
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Reads SeString.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static SeString Read(this Utf8String str) => ReadSeString(&str);

    /// <summary>
    /// Reads Span of bytes into <see langword="string"/>.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string Read(this Span<byte> bytes)
    {
        for(var i = 0; i < bytes.Length; i++)
        {
            if(bytes[i] == 0)
            {
                fixed(byte* ptr = bytes)
                {
                    return Marshal.PtrToStringUTF8((nint)ptr, i);
                }
            }
        }
        fixed(byte* ptr = bytes)
        {
            return Marshal.PtrToStringUTF8((nint)ptr, bytes.Length);
        }
    }
    public static SeString ReadSeString(Utf8String* utf8String)
    {
        if(utf8String != null)
        {
            return SeString.Parse(utf8String->AsSpan());
        }

        return string.Empty;
    }
    #endregion
}
