using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECommons.Interop.WindowFunctionsExtern;

namespace ECommons.Interop
{
    public static class WindowFunctions
    {
        public const int SW_MINIMIZE = 6;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_SHOWNA = 8;

        public static bool TryFindGameWindow(out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;
            while (true)
            {
                hwnd = FindWindowEx(IntPtr.Zero, hwnd, "FFXIVGAME", null);
                if (hwnd == IntPtr.Zero) break;
                GetWindowThreadProcessId(hwnd, out var pid);
                if (pid == Environment.ProcessId) break;
            }
            return hwnd != IntPtr.Zero;
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Environment.ProcessId;
            GetWindowThreadProcessId(activatedHandle, out int activeProcId);

            return activeProcId == procId;
        }
    }
}
