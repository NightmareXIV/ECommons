using FFXIVClientStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.MathHelpers;
/// <summary>
/// Ultimate number union. Offers same performance as using numbers directly.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly unsafe struct Number : IEquatable<Number>
{
    [FieldOffset(0)] private readonly long LongValue;
    [FieldOffset(0)] private readonly ulong ULongValue;
    [FieldOffset(0)] private readonly nint NintValue;
    [FieldOffset(0)] private readonly nuint NUintValue;
    [FieldOffset(0)] private readonly int IntValue;
    [FieldOffset(0)] private readonly uint UIntValue;
    [FieldOffset(0)] private readonly short ShortValue;
    [FieldOffset(0)] private readonly ushort UShortValue;
    [FieldOffset(0)] private readonly sbyte SByteValue;
    [FieldOffset(0)] private readonly byte ByteValue;

    public Number(nint value)
    {
        NintValue = value;
    }

    public Number(nuint value)
    {
        NUintValue = value;
    }

    public Number(long value)
    {
        LongValue = value;
    }

    public Number(int value)
    {
        IntValue = value;
    }

    public Number(short value)
    {
        ShortValue = value;
    }

    public Number(byte value)
    {
        ByteValue = value;
    }
    public Number(ulong value)
    {
        ULongValue = value;
    }

    public Number(uint value)
    {
        UIntValue = value;
    }

    public Number(ushort value)
    {
        UShortValue = value;
    }

    public Number(sbyte value)
    {
        SByteValue = value;
    }

    public static implicit operator byte(Number n) => n.ByteValue;
    public static implicit operator sbyte(Number n) => n.SByteValue;
    public static implicit operator ushort(Number n) => n.UShortValue;
    public static implicit operator short(Number n) => n.ShortValue;
    public static implicit operator uint(Number n) => n.UIntValue;
    public static implicit operator int(Number n) => n.IntValue;
    public static implicit operator ulong(Number n) => n.ULongValue;
    public static implicit operator long(Number n) => n.LongValue;
    public static implicit operator nuint(Number n) => n.NUintValue;
    public static implicit operator nint(Number n) => n.NintValue;
    public static implicit operator Number(nint n) => new(n);
    public static implicit operator Number(nuint n) => new(n);
    public static implicit operator Number(int n) => new(n);
    public static implicit operator Number(uint n) => new(n);
    public static implicit operator Number(short n) => new(n);
    public static implicit operator Number(ushort n) => new(n);
    public static implicit operator Number(long n) => new(n);
    public static implicit operator Number(ulong n) => new(n);
    public static implicit operator Number(byte n) => new(n);
    public static implicit operator Number(sbyte n) => new(n);

    public static bool operator ==(Number left, Number right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Number left, Number right)
    {
        return !(left == right);
    }

    public override readonly string ToString()
    {
        return $"{LongValue}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Number number && Equals(number);
    }

    public bool Equals(Number other)
    {
        return LongValue == other.LongValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LongValue);
    }

    public static Number operator +(Number a, Number b) => new(a.LongValue + b.LongValue);
    public static Number operator -(Number a, Number b) => new(a.LongValue - b.LongValue);
    public static Number operator *(Number a, Number b) => new(a.LongValue * b.LongValue);
    public static Number operator /(Number a, Number b) => new(a.LongValue / b.LongValue);
}
