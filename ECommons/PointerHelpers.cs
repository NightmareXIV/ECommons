using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons
{
    public unsafe class PointerHelpers
    {
        public static T* As<T>(IntPtr ptr) where T:unmanaged
        {
            return (T*)ptr;
        }
    }
}
