using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.UIInput;
/// <summary>
/// Input data.
/// </summary>
public unsafe sealed class InputData : IDisposable
{
    private nint Bytes;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputData"/> class.
    /// </summary>
    private InputData()
    {
        Bytes = Marshal.AllocHGlobal(0x40);
        this.Data = (void**)Bytes;
        if (this.Data == null)
            throw new ArgumentNullException("InputData could not be created, null");

        this.Data[0] = null;
        this.Data[1] = null;
        this.Data[2] = null;
        this.Data[3] = null;
        this.Data[4] = null;
        this.Data[5] = null;
        this.Data[6] = null;
        this.Data[7] = null;
    }

    /// <summary>
    /// Gets the data pointer.
    /// </summary>
    public void** Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputData"/> class.
    /// </summary>
    /// <returns>Input data.</returns>
    public static InputData Empty()
    {
        return new InputData();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputData"/> class.
    /// </summary>
    /// <param name="popupMenu">List popup menu.</param>
    /// <param name="index">Selected index.</param>
    /// <returns>Input data.</returns>
    public static InputData ForPopupMenu(PopupMenu* popupMenu, ushort index)
    {
        var data = new InputData();
        data.Data[0] = popupMenu->List->ItemRendererList[index].AtkComponentListItemRenderer;
        data.Data[2] = (void*)(index | ((ulong)index << 48));
        return data;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Marshal.FreeHGlobal(Bytes);
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~InputData()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
