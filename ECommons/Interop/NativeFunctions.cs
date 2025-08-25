using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;

namespace ECommons.Interop;
public static unsafe  class NativeFunctions
{
    public static ushort LOWORD(uint value) => (ushort)(value & 0xFFFF);
    public static ushort HIWORD(uint value) => (ushort)(value >> 16);
    public static uint MAKEWPARAM(ushort low, ushort high) => (uint)(low | (high << 16));

    [Obsolete("Use TerraFX.Interop.Windows.Windows.SendMessage instead", true)]
    public static  LRESULT SendMessage(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.FindWindowEx instead", true)]
    public static  HWND FindWindowEx(HWND hwndParent, HWND hwndChildAfter, string? lpszClass, string? lpszWindow) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.GetWindowThreadProcessId instead", true)]
    public static uint GetWindowThreadProcessId(HWND hWnd, out uint lpdwProcessId) { lpdwProcessId = 0; return default; }

    [Obsolete("Use TerraFX.Interop.Windows.Windows.GetForegroundWindow instead", true)]
    public static  nint GetForegroundWindow() => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.GetKeyState instead", true)]
    public static  short GetKeyState(int nVirtKey) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.GetAsyncKeyState instead", true)]
    public static  short GetAsyncKeyState(int vKey) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.ShowWindow instead", true)]
    public static  BOOL ShowWindow(HWND hWnd, int nCmdShow) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.ClientToScreen instead", true)]
    public static unsafe  BOOL ClientToScreen(HWND hWnd, POINT* lpPoint) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.SetCursorPos instead", true)]
    public static  BOOL SetCursorPos(int X, int Y) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.SetWindowText instead", true)]
    public static  BOOL SetWindowText(HWND hWnd, string lpString) => default;

    [Obsolete("Use TerraFX.Interop.Windows.Windows.SendInput instead", true)]
    public static unsafe  uint SendInput(uint nInputs, INPUT* pInputs, int cbSize) => default;
}