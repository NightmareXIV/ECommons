using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.CSExtensions;

public unsafe static class InputTimerModuleExtensions
{
    extension(ref InputTimerModule thisRef)
    {
        public float Unk1C
        {
            get
            {
                fixed(InputTimerModule* ptr = &thisRef)
                {
                    return *(float*)(((nint)ptr) + 28);
                }
            }
            set
            {
                fixed(InputTimerModule* ptr = &thisRef)
                {
                    *(float*)(((nint)ptr) + 28) = value;
                }
            }
        }
    }
}
