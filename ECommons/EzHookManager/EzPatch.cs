using Dalamud;
using ECommons.DalamudServices;
using ECommons.LazyDataHelpers;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzHookManager;
public class EzPatch : IDisposable
{
    internal static List<EzPatch> References
    {
        get
        {
            if(field == null)
            {
                field = [];
                Purgatory.Add(static () => field.ToArray().Each(x => x.Dispose()));
            }
            return field;
        }
    }
    private bool Disposed = false;
    public bool Enabled { get; private set; } = false;
    private Data PatchData;
    private bool Silent = false;
    public nint Address { get; private set; }
    public EzPatch(nint addr, Data patchData, bool autoEnable = true, bool silent = false)
    {
        Initialize(addr, patchData, autoEnable, silent);
    }

    public EzPatch(string signature, nint offset, Data patchData, bool autoEnable = true, bool silent = false)
    {
        if(Svc.SigScanner.TryScanText(signature, out var addr))
        {
             Initialize(addr + offset, patchData, autoEnable, silent);
        }
    }

    private void Initialize(nint addr, Data patchData, bool autoEnable = true, bool silent = false)
    {
        try
        {
            Silent = silent;
            if(patchData.OriginalData.Count == 0)
            {
                if(!silent) PluginLog.Debug($"Reading {patchData.PatchData.Count} bytes from {addr:X} and using it as original data for patch {patchData.PatchData}");
                if(SafeMemory.ReadBytes(addr, patchData.PatchData.Count, out var result))
                {
                    if(!silent) PluginLog.Debug($"Successfully read {result.ToHexString()}");
                    patchData = new(result, patchData.PatchData);
                }
            }
            if(patchData.OriginalData.Count > 0)
            {
                if(SafeMemory.ReadBytes(addr, patchData.OriginalData.Count, out var result))
                {
                    if(result.SequenceEqual(patchData.OriginalData))
                    {
                        Address = addr;
                        PatchData = patchData;
                        if(!silent) PluginLog.Debug($"Patch created at {Address} with {PatchData}");
                        References.Add(this);
                        if(autoEnable) Enable();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not create patch {patchData}: unexpected data");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"Could not create patch {patchData}: original data not found");
            }
        }
        catch(Exception e)
        {
            if(silent)
            {
                PluginLog.Error($"An error occurred while enabling patch");
            }
            else
            {
                PluginLog.Error($"An error occurred while enabling patch {patchData}");
            }
            e.Log();
            this.Dispose();
        }
    }

    public void Enable()
    {
        try
        {
            if(Disposed)
            {
                PluginLog.Error("Can not enable disposed patch");
                return;
            }
            if(!Enabled)
            {
                SafeMemory.WriteBytes(Address, [..this.PatchData.PatchData]);
                Enabled = true;
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
    }

    public void Disable()
    {
        try
        {
            if(Enabled)
            {
                SafeMemory.WriteBytes(Address, [.. this.PatchData.OriginalData]);
                Enabled = false;
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
    }

    public virtual void Dispose()
    {
        if(!Silent) PluginLog.Debug($"Disposing patch at {Address:X}");
        Disable();
        Disposed = true;
        References.Remove(this);
    }

    public class Data
    {
        public readonly IReadOnlyList<byte> OriginalData;
        public readonly IReadOnlyList<byte> PatchData;

        public Data(IReadOnlyList<byte> originalData, IReadOnlyList<byte> patchData)
        {
            if(originalData.Count < patchData.Count)
            {
                throw new InvalidOperationException("Original data must be of the same or higher length than patch data");
            }
            OriginalData = originalData;
            PatchData = patchData;
        }
        public Data(IReadOnlyList<byte> patchData)
        {
            OriginalData = (byte[])[];
            PatchData = patchData;
        }

        public Data(string originalData, string patchData) : this(GenericHelpers.ParseByteArray(originalData), GenericHelpers.ParseByteArray(patchData))
        {
        }

        public Data(string patchData) : this(GenericHelpers.ParseByteArray(patchData))
        {
        }

        public override string ToString()
        {
            return $"EzPatch Data({OriginalData?.ToHexString()} -> {PatchData?.ToHexString()})";
        }
    }
}