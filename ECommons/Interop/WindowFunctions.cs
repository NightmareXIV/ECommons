using Dalamud.Plugin.Ipc.Exceptions;
using ECommons.LazyDataHelpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using TerraFX.Interop.Windows;

namespace ECommons.Interop;

public static unsafe class WindowFunctions
{
    public const int SW_MINIMIZE = 6;
    public const int SW_FORCEMINIMIZE = 11;
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;
    public const int SW_SHOWNA = 8;

    private static readonly Lock FFXIVClassNamePtrLock = new();
    public static ushort* FFXIVClassNamePtr
    {
        get
        {
            if(field == null)
            {
                lock(FFXIVClassNamePtrLock)
                {
                    if(field == null)
                    {
                        var str = "FFXIVGAME\0";
                        var size = str.Length * sizeof(char);
                        var ptr = Marshal.AllocHGlobal(size);
                        fixed(char* strPtr = str)
                        {
                            Buffer.MemoryCopy(strPtr, (void*)ptr, size, size);
                        }
                        field = (ushort*)ptr;
                        Purgatory.Add(() => Marshal.FreeHGlobal(ptr));
                    }
                }
            }
            return field;
        }
    } = null;

    public static bool TryFindGameWindow(out HWND hwnd)
    {
        hwnd = HWND.NULL;
        var prev = HWND.NULL;

        while(true)
        {
            prev = TerraFX.Interop.Windows.Windows.FindWindowEx(HWND.NULL, prev, FFXIVClassNamePtr, null);
            if(prev == HWND.NULL)
                break;

            uint pid;
            _ = TerraFX.Interop.Windows.Windows.GetWindowThreadProcessId(prev, &pid);
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
        var activatedHandle = TerraFX.Interop.Windows.Windows.GetForegroundWindow();
        if(activatedHandle == HWND.NULL)
        {
            return false;
        }

        var procId = (uint)Environment.ProcessId;
        uint activeProcId;
        TerraFX.Interop.Windows.Windows.GetWindowThreadProcessId(activatedHandle, &activeProcId);

        return activeProcId == procId;
    }

    public static bool SendKeypress(int keycode)
    {
        if(TryFindGameWindow(out var hwnd))
        {
            TerraFX.Interop.Windows.Windows.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)keycode, (LPARAM)0);
            TerraFX.Interop.Windows.Windows.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)keycode, (LPARAM)0);
            return true;
        }
        return false;
    }
}
