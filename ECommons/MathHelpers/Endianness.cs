using System;
using System.Buffers.Binary;

namespace ECommons.MathHelpers;

public static class Endianness
{
    [Obsolete("Use BinaryPrimitives.ReverseEndianness")]
    public static uint SwapBytes(this uint x)
    {
        return ((x & 0x000000ff) << 24) +
               ((x & 0x0000ff00) << 8) +
               ((x & 0x00ff0000) >> 8) +
               ((x & 0xff000000) >> 24);
    }
}
