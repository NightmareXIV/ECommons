using Dalamud.Bindings.ImGui;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ECommons.ImGuiMethods;
#nullable disable

// ImGui extra functionality related with Drag and Drop
public static class ImGuiDragDrop
{
    // TODO: review
    // can now pass refs with Unsafe.AsRef

    public static unsafe void SetDragDropPayload<T>(string type, T data, ImGuiCond cond = 0)
    where T : unmanaged
    {
        var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref data, 1));
        ImGui.SetDragDropPayload(type, span, cond);
    }

    public static unsafe bool AcceptDragDropPayload<T>(string type, out T payload, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
    where T : unmanaged
    {
        ImGuiPayload* pload = ImGui.AcceptDragDropPayload(type, flags);
        payload = (pload != null) ? Unsafe.Read<T>(pload->Data) : default;
        return pload != null;
    }

    public static unsafe void SetDragDropPayload(string type, string data, ImGuiCond cond = 0)
    {
        Span<byte> utf8Bytes = stackalloc byte[Encoding.UTF8.GetByteCount(data)];
        Encoding.UTF8.GetBytes(data, utf8Bytes);
        ReadOnlySpan<byte> span = utf8Bytes;
        ImGui.SetDragDropPayload(type, span, cond);
    }

    public static unsafe bool AcceptDragDropPayload(string type, out string payload, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
    {
        ImGuiPayload* pload = ImGui.AcceptDragDropPayload(type, flags);
        payload = (pload != null) ? Encoding.Default.GetString((byte*)pload->Data, pload->DataSize) : null;
        return pload != null;
    }
}
