using Dalamud.Hooking;
using ECommons.Logging;
using ECommons.DalamudServices;

namespace ECommons.EzHookManager
{
    /// <summary>
    /// A wrapper around Dalamud hook. Achieves 2 goals:
    /// - Auto-disposing all undisposed hooks upon plugin unload;
    /// - Instead of disabling, completely disposes hook, restoring original code in memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EzHook<T> where T:System.Delegate
    {
        public nint Address { get; private set; }
        public T Detour { get; private set; }
        internal Hook<T> HookDelegate = null;
        /// <summary>
        /// Delegate that points to provided address which you can call even without enabling your hook. 
        /// </summary>
        public T Delegate { get; private set; }

        public EzHook(string sig, T detour, bool autoEnable = true, int offset = 0)
        {
            var addr = Svc.SigScanner.ScanText(sig) + offset;
            Setup(addr, detour, autoEnable);
        }

        public EzHook(nint address, T detour, bool autoEnable = true)
        {
            Setup(address, detour, autoEnable);
        }

        void Setup(nint address, T detour, bool autoEnable)
        {
            Address = address;
            Delegate = EzDelegate.Get<T>(address);
            Log($"Configured {typeof(T).FullName} at {address:X16}");
            Detour = detour;
            if (autoEnable) HookDelegate.Enable();
        }

        public void Enable()
        {
            if (HookDelegate == null)
            {
                Log($"Creating hook {typeof(T).FullName}");
                HookDelegate = Svc.Hook.HookFromAddress(Address, Detour);
                EzHookCommon.RegisteredHooks.Add(HookDelegate);
            }
            else if (!HookDelegate.IsEnabled)
            {
                PluginLog.Error($"[EzHook] Hook is created but not enabled, this should not ever happen. Please report at https://github.com/NightmareXIV/ECommons/issues with logs and, if you are developer, source code.");
                Log($"Enabling hook {typeof(T).FullName} at {Address:X16}");
            }
        }

        /// <summary>
        /// Disabling EzHook disposes underlying hook. 
        /// </summary>
        public void Disable()
        {
            if(HookDelegate != null)
            {
                HookDelegate.Dispose();
                EzHookCommon.RegisteredHooks.Remove(HookDelegate);
                HookDelegate = null;
                Log($"Disposing hook {typeof(T).FullName} at {Address:X16}");
            }
        }

        public bool IsEnabled => HookDelegate != null && HookDelegate.IsEnabled;
        /// <summary>
        /// Calls original function as if it was unhooked if hook is enabled; calls original Delegate if hook is disabled.
        /// </summary>
        public T Original => HookDelegate?.Original ?? Delegate;
        static void Log(string s) => PluginLog.Debug($"[EzHook] {s}");
    } 
}
