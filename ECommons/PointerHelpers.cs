using FFXIVClientStructs.FFXIV.Common.Lua;
using System;

namespace ECommons;

public static unsafe class PointerHelpers
{
    public static TResult Reinterpret<TValue, TResult>(this TValue value) where TResult : unmanaged where TValue:unmanaged
    {
        return *(TResult*)&value;
    }

    public static T* As<T>(this IntPtr ptr) where T : unmanaged
    {
        return (T*)ptr;
    }
}
