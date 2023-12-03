using Dalamud.Hooking;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzHookManager
{
    public class EzHook<T> where T:System.Delegate
    {
        internal Hook<T> HookDelegate;
        internal T DetourDelegate;

        public EzHook(string sig, T detour, bool autoEnable = true, int offset = 0)
        {
            DetourDelegate = detour;
            HookDelegate = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText(sig) + offset, DetourDelegate);
            EzHookCommon.RegisteredHooks.Add(HookDelegate);
            if (autoEnable) HookDelegate.Enable();
        }

        public EzHook(nint address, T detour, bool autoEnable = true)
        {
            DetourDelegate = detour;
            HookDelegate = Svc.Hook.HookFromAddress(address, DetourDelegate);
            EzHookCommon.RegisteredHooks.Add(HookDelegate);
            if (autoEnable) HookDelegate.Enable();
        }

        public void Disable() => HookDelegate.Disable();
        public void Enable() => HookDelegate.Enable();
        public bool IsEnabled => HookDelegate.IsEnabled;
        public T Detour => DetourDelegate;
        public T Original => HookDelegate.Original;
        public void Dispose()
        {
            HookDelegate.Dispose();
            EzHookCommon.RegisteredHooks.Remove(HookDelegate);
        }
    } 
}
