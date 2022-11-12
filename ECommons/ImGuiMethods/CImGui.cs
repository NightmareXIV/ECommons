using System;
using System.Runtime.InteropServices;

namespace ECommons.ImGuiMethods;

public class CImGui
{
    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern void igBringWindowToDisplayFront(IntPtr ptr);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern void igBringWindowToDisplayBack(IntPtr ptr);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr igGetCurrentWindow();
}
