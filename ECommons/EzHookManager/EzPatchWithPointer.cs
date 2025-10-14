using ECommons.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace ECommons.EzHookManager;
public unsafe class EzPatchWithPointer<T> : EzPatch, IDisposable where T : unmanaged
{
    public readonly T* PointerRaw;

    public T PointerValue
    {
        get
        {
            return *PointerRaw;
        }
        set
        {
            if(Enabled)
            {
                *PointerRaw = value;
            }
        }
    }

    private bool RestoreOriginal;

    private uint? OldProtect;

    public EzPatchWithPointer(nint addr, Data patchData, nint pointerOffset, bool autoEnable = true, bool silent = false, bool restoreOriginal = false) : base(addr, patchData, autoEnable, silent)
    {
        PointerRaw = (T*)(Address + pointerOffset);
        RestoreOriginal = restoreOriginal;
        InitializePointer();
    }

    public EzPatchWithPointer(string signature, nint offset, Data patchData, nint pointerOffset, bool autoEnable = true, bool silent = false, bool restoreOriginal = false) : base(signature, offset, patchData, autoEnable, silent)
    {
        PointerRaw = (T*)(Address + pointerOffset);
        RestoreOriginal = restoreOriginal;
        InitializePointer();
    }

    private void InitializePointer()
    {
        try
        {
            uint oldProtectTemp = 0;
            var result = TerraFX.Interop.Windows.Windows.VirtualProtectEx((HANDLE)Process.GetCurrentProcess().Handle, PointerRaw, (nuint)sizeof(T), PAGE.PAGE_EXECUTE_READWRITE, &oldProtectTemp) != 0;
            if(!result)
            {
                var errorCode = Marshal.GetLastWin32Error();
                PluginLog.Error($"Failed to change memory protection. Error code: {errorCode} (0x{errorCode:X})");
            }
            OldProtect = oldProtectTemp;
        }
        catch(Exception e)
        {
            e.Log();
        }
    }

    public override void Dispose()
    {
        try
        {
            if(OldProtect != null && RestoreOriginal)
            {
                uint currentProtect = 0;

                var result = TerraFX.Interop.Windows.Windows.VirtualProtectEx((HANDLE)Process.GetCurrentProcess().Handle, PointerRaw, (nuint)sizeof(T), OldProtect.Value, &currentProtect) != 0;

                if(!result)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    PluginLog.Error($"Failed to restore memory protection. Error code: {errorCode} (0x{errorCode:X})");
                }

                if(currentProtect != OldProtect)
                {
                    PluginLog.Error($"Memory protection was changed by something else. Expected: 0x{OldProtect:X}, Found: 0x{currentProtect:X}. Restoration cancelled.");

                    uint revertDummy = 0;
                    TerraFX.Interop.Windows.Windows.VirtualProtectEx((HANDLE)Process.GetCurrentProcess().Handle, PointerRaw, (nuint)sizeof(T), currentProtect, &revertDummy);

                }
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
        base.Dispose();
    }
}