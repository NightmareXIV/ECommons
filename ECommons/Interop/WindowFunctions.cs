using System;
using TerraFX.Interop.Windows;

namespace ECommons.Interop;

public static class WindowFunctions
{
    public const int SW_MINIMIZE = 6;
    public const int SW_FORCEMINIMIZE = 11;
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;
    public const int SW_SHOWNA = 8;


    public static bool TryFindGameWindow(out HWND hwnd)
    {
        hwnd = HWND.NULL;
        var prev = HWND.NULL;

        while(true)
        {
            prev = NativeFunctions.FindWindowEx(HWND.NULL, prev, "FFXIVGAME", null);
            if(prev == HWND.NULL)
                break;

            NativeFunctions.GetWindowThreadProcessId(prev, out var pid);
            if(pid == Environment.ProcessId)
            {
                hwnd = prev;
                break;
            }
        }

        return hwnd != HWND.NULL;
    }

    /// <summary>Returns true if the current application has focus, false otherwise</summary>
    public static bool ApplicationIsActivated()
    {
        var activatedHandle = NativeFunctions.GetForegroundWindow();
        if(activatedHandle == HWND.NULL)
        {
            return false;
        }

        var procId = (uint)Environment.ProcessId;
        NativeFunctions.GetWindowThreadProcessId(activatedHandle, out var activeProcId);

        return activeProcId == procId;
    }

    public static bool SendKeypress(int keycode)
    {
        if(TryFindGameWindow(out var hwnd))
        {
            NativeFunctions.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)keycode, (LPARAM)0);
            NativeFunctions.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)keycode, (LPARAM)0);
            return true;
        }
        return false;
    }
}
