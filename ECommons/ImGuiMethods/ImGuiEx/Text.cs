using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;

namespace ECommons.ImGuiMethods;
public static partial class ImGuiEx
{
    public static void Text(string s) => ImGui.TextUnformatted(s);

    public static void Text(Vector4? col, string s)
    {
        if(col is { } c)
            Text(c, s);
        else
            Text(s);
    }

    public static void Text(ImFontPtr font, string s)
    {
        using var _ = ImRaii.PushFont(font);
        Text(s);
    }

    public static void Text(Vector4 col, string s)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, col);
        Text(s);
    }

    public static void Text(uint col, string s)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, col);
        Text(s);
    }

    public static void Text(EzColor col, string s) => Text(col.Vector4, s);

    public static void Text(Vector4? col, ImFontPtr? font, string s)
    {
        using var f = font.HasValue ? ImRaii.PushFont(font.Value) : null;
        using var c = col.HasValue ? ImRaii.PushColor(ImGuiCol.Text, col.Value) : null;
        Text(s);
    }

    public static void Text(uint col, ImFontPtr font, string s)
    {
        using var f = ImRaii.PushFont(font);
        using var c = ImRaii.PushColor(ImGuiCol.Text, col);
        Text(s);
    }

    /// <summary>
    /// Aligns text vertically to a standard size button.
    /// </summary>
    /// <param name="col">Color</param>
    /// <param name="s">Text</param>
    public static void TextV(Vector4? col, string s)
    {
        using var _ = col.HasValue ? ImRaii.PushColor(ImGuiCol.Text, col.Value) : null;
        TextV(s);
    }

    /// <summary>
    /// Aligns text vertically to a standard size button.
    /// </summary>
    /// <param name="s">Text</param>
    public static void TextV(string s)
    {
        ImGui.AlignTextToFramePadding();
        Text(s);
    }

    public static void TextWrapped(string s)
    {
        using var _ = ImRaii.TextWrapPos(0);
        Text(s);
    }

    public static void TextWrapped(Vector4? col, string s)
    {
        using var _ = ImRaii.TextWrapPos(0);
        Text(col, s);
    }

    public static void TextWrapped(uint col, string s)
    {
        using var _ = ImRaii.TextWrapPos(0);
        Text(col, s);
    }

    /// <inheritdoc cref="TextCopy(string)"/>
    public static void TextCopy(Vector4 col, string text)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, col);
        TextCopy(text);
    }

    /// <summary>
    /// Displays text that will also be copied to clipboard if clicked.
    /// </summary>
    public static void TextCopy(string text)
    {
        Text(text);
        if(HoveredAndClicked())
            GenericHelpers.Copy(text);
    }

    /// <summary>
    /// Displays wrapped text that will also be copied to clipboard if clicked.
    /// </summary>
    public static void TextWrappedCopy(string text)
    {
        TextWrapped(text);
        if(HoveredAndClicked())
            GenericHelpers.Copy(text);
    }

    /// <inheritdoc cref="TextWrappedCopy(string)"/>
    public static void TextWrappedCopy(Vector4 col, string text)
    {
        TextWrapped(col, text);
        if(HoveredAndClicked())
            GenericHelpers.Copy(text);
    }

    public static void TextCentered(string text)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X / 2 - ImGui.CalcTextSize(text).X / 2);
        Text(text);
    }

    public static void TextCentered(Vector4 col, string text)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, col);
        TextCentered(text);
    }

    public static void TextCentered(Vector4? col, string text)
    {
        if(col is { } c)
            TextCentered(c, text);
        else
            TextCentered(text);
    }

    public static void CenterColumnText(string text, bool underlined = false)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() * 0.5f) - (ImGui.CalcTextSize(text).X * 0.5f));
        if(underlined)
            TextUnderlined(text);
        else
            Text(text);
    }

    public static void CenterColumnText(Vector4 colour, string text, bool underlined = false)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, colour);
        CenterColumnText(text, underlined);
    }

    public static void TextUnderlined(uint color, string text)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, color);
        TextUnderlined(text);
    }

    public static void TextUnderlined(Vector4 color, string text)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, color);
        TextUnderlined(text);
    }

    public static void TextUnderlined(string text)
    {
        var size = ImGui.CalcTextSize(text);
        var cur = ImGui.GetCursorScreenPos();
        cur.Y += size.Y;
        ImGui.GetWindowDrawList().PathLineTo(cur);
        cur.X += size.X;
        ImGui.GetWindowDrawList().PathLineTo(cur);
        ImGui.GetWindowDrawList().PathStroke(ImGuiColors.DalamudWhite.ToUint());
        Text(text);
    }
}
