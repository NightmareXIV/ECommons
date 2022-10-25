using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public class CImGui
    {
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToDisplayFront(IntPtr ptr);

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToDisplayBack(IntPtr ptr);

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr igGetCurrentWindow();
    }
}
