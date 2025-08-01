using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;

namespace ECommons.Interop;
public static unsafe partial class NativeFunctions
{
    public static ushort LOWORD(uint value) => (ushort)(value & 0xFFFF);
    public static ushort HIWORD(uint value) => (ushort)(value >> 16);
    public static uint MAKEWPARAM(ushort low, ushort high) => (uint)(low | (high << 16));

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial LRESULT SendMessage(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial HWND FindWindowEx(HWND hwndParent, HWND hwndChildAfter, string lpszClass, string? lpszWindow);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint GetWindowThreadProcessId(HWND hWnd, out uint lpdwProcessId);

    [LibraryImport("user32.dll")]
    public static partial HWND GetForegroundWindow();

    [LibraryImport("user32.dll")]
    public static partial short GetKeyState(int nVirtKey);

    [LibraryImport("user32.dll")]
    public static partial short GetAsyncKeyState(int vKey);

}