using ECommons.Interop;
using ECommons.Logging;
using PInvoke;

namespace ECommons.Automation;

public static partial class WindowsKeypress
{
		public static bool SendKeypress(LimitedKeys key) => SendKeypress((int)key);
		public static bool SendMousepress(LimitedKeys key) => SendKeypress((int)key);

		public static bool SendKeypress(int key)
    {
        if (WindowFunctions.TryFindGameWindow(out var h))
        {
            InternalLog.Verbose($"Sending key {key}");
            User32.SendMessage(h, User32.WindowMessage.WM_KEYDOWN, key, 0);
            User32.SendMessage(h, User32.WindowMessage.WM_KEYUP, key, 0);
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
        if (WindowFunctions.TryFindGameWindow(out var h))
        {
            if (key == (1 | 4)) //xbutton1
            {
                var wparam = MAKEWPARAM(0, 0x0001);
                User32.SendMessage(h, User32.WindowMessage.WM_XBUTTONDOWN, wparam, 0);
                User32.SendMessage(h, User32.WindowMessage.WM_XBUTTONUP, wparam, 0);
            }
            else if (key == (2 | 4)) //xbutton2
            {
                var wparam = MAKEWPARAM(0, 0x0002);
                User32.SendMessage(h, User32.WindowMessage.WM_XBUTTONDOWN, wparam, 0);
                User32.SendMessage(h, User32.WindowMessage.WM_XBUTTONUP, wparam, 0);
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
    internal static int MAKEWPARAM(int l, int h)
    {
        return (l & 0xFFFF) | (h << 16);
    }
}
