using Dalamud.Hooking;
using ECommons.Logging;
using ECommons.DalamudServices;
using Dalamud;
using System.Linq;

namespace ECommons.EzHookManager;
#nullable disable

/// <summary>
/// A wrapper around Dalamud hook. Achieves 2 goals:
/// - Auto-disposing all undisposed hooks upon plugin unload;
/// - Lazy hooking and completely disposing hook upon disabling;
/// - Increasing transparency to developer, indicating that Dalamud's disable method doesn't completely disables it and just pauses detour execution.
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
        if (autoEnable) Enable();
    }


    public void Enable()
    {
        if (HookDelegate == null)
        {
            byte[] orig = null;
            Log($"Creating hook {typeof(T).FullName} at {Address:X16}");
            if (EzHookCommon.TrackMemory > 0) SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out orig);
            HookDelegate = Svc.Hook.HookFromAddress(Address, Detour);
            HookDelegate.Enable();
            if (EzHookCommon.TrackMemory > 0 && SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out var changed) && orig != null)
            {
                PluginLog.Debug($"   Before: {orig.Select(x => $"{x:X2}").Print(" ")}");
                PluginLog.Debug($"    After: {changed.Select(x => $"{x:X2}").Print(" ")}");
            }
            EzHookCommon.RegisteredHooks.Add(HookDelegate);
        }
        else if (!HookDelegate.IsEnabled)
        {
            Log($"Enabling hook {typeof(T).FullName} at {Address:X16}");
            HookDelegate.Enable();
        }
    }

    public void Pause()
    {
        byte[] orig = null;
        if (HookDelegate != null)
        {
            Log($"Disabling hook {typeof(T).FullName} at {Address:X16}");
            if (EzHookCommon.TrackMemory > 0) SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out orig);
            HookDelegate.Disable();
            if (EzHookCommon.TrackMemory > 0 && SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out var changed) && orig != null)
            {
                PluginLog.Debug($"   Before: {orig.Select(x => $"{x:X2}").Print(" ")}");
                PluginLog.Debug($"    After: {changed.Select(x => $"{x:X2}").Print(" ")}");
            }
        }
    }

    /// <summary>
    /// Disabling EzHook disposes underlying hook. 
    /// </summary>
    public void Disable()
    {
        if(HookDelegate != null)
        {
            byte[] orig = null;
            Log($"Disposing hook {typeof(T).FullName} at {Address:X16}");
            if (EzHookCommon.TrackMemory > 0) SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out orig);
            HookDelegate.Dispose();
            EzHookCommon.RegisteredHooks.Remove(HookDelegate);
            HookDelegate = null;
            if (EzHookCommon.TrackMemory > 0 && SafeMemory.ReadBytes(Address, EzHookCommon.TrackMemory, out var changed) && orig != null)
            {
                PluginLog.Debug($"   Before: {orig.Select(x => $"{x:X2}").Print(" ")}");
                PluginLog.Debug($"    After: {changed.Select(x => $"{x:X2}").Print(" ")}");
            }
        }
    }

    public bool IsEnabled => HookDelegate != null && HookDelegate.IsEnabled;
    public bool IsCreated => HookDelegate != null;
    /// <summary>
    /// Calls original function as if it was unhooked if hook is enabled; calls original Delegate if hook is disabled.
    /// </summary>
    public T Original => HookDelegate?.Original ?? Delegate;
    static void Log(string s) => PluginLog.Debug($"[EzHook] {s}");
} 
