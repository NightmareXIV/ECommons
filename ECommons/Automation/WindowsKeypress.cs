using ECommons.Interop;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using TerraFX.Interop.Windows;
using VirtualKey = Dalamud.Game.ClientState.Keys.VirtualKey;

namespace ECommons.Automation;

public static partial class WindowsKeypress
{
    public static bool SendKeypress(LimitedKeys key) => SendKeypress((int)key);
    public static bool SendMousepress(LimitedKeys key) => SendKeypress((int)key);

    public static bool SendKeypress(int key)
    {
        if(WindowFunctions.TryFindGameWindow(out var h))
        {
            InternalLog.Verbose($"Sending key {key}");
            WPARAM wParam = new WPARAM((uint)key);
            LPARAM lParam = new LPARAM(0);
            NativeFunctions.SendMessage(h, WM.WM_KEYDOWN, wParam, lParam);
            NativeFunctions.SendMessage(h, WM.WM_KEYUP, wParam, lParam);
            return true;
        }
        else
        {
            PluginLog.Error("Couldn't find game window!");
        }
        return false;
    }
    public static void SendMousepress(int key)
    {
        if(WindowFunctions.TryFindGameWindow(out var h))
        {
            if(key == (1 | 4)) //xbutton1
            {
                uint rawWParam = NativeFunctions.MAKEWPARAM(0, 0x0001);
                HWND hwnd = h;
                WPARAM wp = new WPARAM(rawWParam);
                LPARAM lp = new LPARAM(0);

                NativeFunctions.SendMessage(hwnd, WM.WM_XBUTTONDOWN, wp, lp);
                NativeFunctions.SendMessage(hwnd, WM.WM_XBUTTONUP, wp, lp);
            }
            else if(key == (2 | 4)) //xbutton2
            {
                uint rawWParam = NativeFunctions.MAKEWPARAM(0, 0x0002);
                HWND hwnd = h;
                WPARAM wp = new WPARAM(rawWParam);
                LPARAM lp = new LPARAM(0);

                NativeFunctions.SendMessage(hwnd, WM.WM_XBUTTONDOWN, wp, lp);
                NativeFunctions.SendMessage(hwnd, WM.WM_XBUTTONUP, wp, lp);
            }
            else
            {
                PluginLog.Error($"Invalid key: {key}");
            }
        }
        else
        {
            PluginLog.Error("Couldn't find game window!");
        }
    }

    public static bool SendKeypress(VirtualKey key, IEnumerable<VirtualKey>? modifiers) => SendKeypress((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeypress(LimitedKeys key, IEnumerable<LimitedKeys>? modifiers) => SendKeypress((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeypress(int key, IEnumerable<int>? modifiers)
    {
        if(WindowFunctions.TryFindGameWindow(out var hwnd))
        {
            if(modifiers is { })
                foreach(var mod in modifiers)
                    NativeFunctions.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)mod, IntPtr.Zero);

            NativeFunctions.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)key, IntPtr.Zero);
            NativeFunctions.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)key, IntPtr.Zero);

            if(modifiers is { })
                foreach(var mod in modifiers)
                    NativeFunctions.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)mod, IntPtr.Zero);
            return true;
        }
        PluginLog.Error("Couldn't find game window!");
        return false;
    }

    public static bool SendKeyHold(VirtualKey key, IEnumerable<VirtualKey>? modifiers) => SendKeyHold((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeyHold(LimitedKeys key, IEnumerable<LimitedKeys>? modifiers) => SendKeyHold((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeyHold(int key, IEnumerable<int>? modifiers)
    {
        if(WindowFunctions.TryFindGameWindow(out var hwnd))
        {
            if(modifiers is { })
                foreach(var mod in modifiers)
                    NativeFunctions.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)mod, IntPtr.Zero);

            NativeFunctions.SendMessage(hwnd, WM.WM_KEYDOWN, (WPARAM)key, IntPtr.Zero);
            return true;
        }
        PluginLog.Error("Couldn't find game window!");
        return false;
    }

    public static bool SendKeyRelease(VirtualKey key, IEnumerable<VirtualKey>? modifiers) => SendKeyRelease((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeyRelease(LimitedKeys key, IEnumerable<LimitedKeys>? modifiers) => SendKeyRelease((int)key, modifiers?.SelectMulti(k => (int)k));
    public static bool SendKeyRelease(int key, IEnumerable<int>? modifiers)
    {
        if(WindowFunctions.TryFindGameWindow(out var hwnd))
        {
            if(modifiers is { })
                foreach(var mod in modifiers)
                    NativeFunctions.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)mod, IntPtr.Zero);

            NativeFunctions.SendMessage(hwnd, WM.WM_KEYUP, (WPARAM)key, IntPtr.Zero);
            return true;
        }
        PluginLog.Error("Couldn't find game window!");
        return false;
    }
}
