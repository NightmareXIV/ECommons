using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.CSExtensions;

public unsafe static class AtkEventStateExtensions
{
    extension(ref AtkEventState thisRef)
    {
        public byte UnkFlags3
        {
            get
            {
                fixed(AtkEventState* ptr = &thisRef)
                {
                    return *(byte*)(((nint)ptr) + 3);
                }
            }
            set
            {
                fixed(AtkEventState* ptr = &thisRef)
                {
                    *(byte*)(((nint)ptr) + 3) = value;
                }
            }
        }
    }
}
