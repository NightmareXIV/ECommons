using ECommons.DalamudServices;
using System.Runtime.InteropServices;

namespace ECommons.EzHookManager;
public static class EzDelegate
{
    public static T Get<T>(string sig, int offset = 0)
    {
        return Marshal.GetDelegateForFunctionPointer<T>(Svc.SigScanner.ScanText(sig) + offset);
    }
    public static T Get<T>(nint address)
    {
        return Marshal.GetDelegateForFunctionPointer<T>(address);
    }
}
