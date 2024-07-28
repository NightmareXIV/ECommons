using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers;
public unsafe class AtkEventDataBuilder
{
    public readonly AtkEventData Data;

    public AtkEventDataBuilder()
    {
        Data = new();
    }

    public AtkEventDataBuilder Write<T>(int pos, T data) where T : unmanaged
    {
        fixed(AtkEventData* eventDataPtr = &Data)
        {
            var ptr = ((nint)eventDataPtr) + pos;
            *(T*)ptr = data;
        }
        return this;
    }

    public AtkEventData Build() => Data;
}
