using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ImGuiNET;
using System.Numerics;

namespace ECommons.ImGuiMethods;
public static partial class ImGuiEx
{
    public static void Text(string s)
    {
        ImGui.TextUnformatted(s);
    }

    public static void Text(ImFontPtr font, string s)
    {
        ImGui.PushFont(font);
        ImGui.TextUnformatted(s);
        ImGui.PopFont();
    }

    public static void Text(Vector4 col, string s)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
    }

    public static void Text(Vector4? col, ImFontPtr? font, string s)
    {
        if(font != null) ImGui.PushFont(font.Value);
        if(col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        ImGui.TextUnformatted(s);
        if(col != null) ImGui.PopStyleColor();
        if(font != null) ImGui.PopFont();
    }

    public static void Text(uint col, string s)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
    }

    public static void Text(uint col, ImFontPtr font, string s)
    {
        ImGui.PushFont(font);
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
        ImGui.PopFont();
    }

    public static void Text(EzColor col, string s)
    {
        Text(col.Vector4, s);
    }

    public static void Icon(FontAwesomeIcon icon) => IconWithText(ImGui.GetStyle().Colors[(int)ImGuiCol.Text], icon);
    public static void Icon(Vector4 col, FontAwesomeIcon icon) => IconWithText(col, icon);
    public static void IconWithText(FontAwesomeIcon icon, string s) => IconWithText(ImGui.GetStyle().Colors[(int)ImGuiCol.Text], icon, s);
    public static void IconWithText(Vector4 col, FontAwesomeIcon icon, string s) => IconWithText(col, icon, s, null);
    public static void IconWithTooltip(FontAwesomeIcon icon, string tooltip) => IconWithText(ImGui.GetStyle().Colors[(int)ImGuiCol.Text], icon, null, tooltip);
    public static void IconWithTooltip(Vector4 col, FontAwesomeIcon icon, string? tooltip = null) => IconWithText(col, icon, null, tooltip);
    public static void IconWithText(Vector4 col, FontAwesomeIcon icon, string? s = null, string? tooltip = null)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(icon.ToIconString());
        ImGui.PopFont();
        ImGui.SameLine();
        if(s != null)
            ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();

        if(tooltip != null)
            Tooltip(tooltip);
    }

    public static void CodeText(string s)
    {
        ImGui.PushFont(UiBuilder.MonoFont);
        ImGui.TextUnformatted(s);
        ImGui.PopFont();
    }

    /// <summary>
    /// Aligns text vertically to a standard size button.
    /// </summary>
    /// <param name="col">Color</param>
    /// <param name="s">Text</param>
    public static void TextV(Vector4? col, string s)
    {
        if(col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        ImGuiEx.TextV(s);
        if(col != null) ImGui.PopStyleColor();
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

    public static void Text(Vector4? col, string text)
    {
        if(col == null)
        {
            Text(text);
        }
        else
        {
            Text(col.Value, text);
        }
    }


    public static void TextWrapped(string s)
    {
        ImGui.PushTextWrapPos();
        ImGui.TextUnformatted(s);
        ImGui.PopTextWrapPos();
    }

    public static void TextWrapped(Vector4? col, string s)
    {
        ImGui.PushTextWrapPos(0);
        ImGuiEx.Text(col, s);
        ImGui.PopTextWrapPos();
    }

    public static void TextWrapped(uint col, string s)
    {
        ImGui.PushTextWrapPos();
        ImGuiEx.Text(col, s);
        ImGui.PopTextWrapPos();
    }

    /// <inheritdoc cref="TextCopy(string, string)"/>
    public static void TextCopy(Vector4 col, string text, string? copyText = null)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        TextCopy(text, copyText);
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Displays text that will also be copied to clipboard if clicked.
    /// </summary>
    public static void TextCopy(string displayText, string? copyText = null)
    {
        copyText ??= displayText;
        ImGui.TextUnformatted(displayText);
        if(ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            GenericHelpers.Copy(copyText);
        }
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
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        TextCentered(text);
        ImGui.PopStyleColor();
    }

    public static void TextCentered(Vector4? col, string text)
    {
        if(col != null)
        {
            TextCentered(col.Value, text);
        }
        else
        {
            TextCentered(text);
        }
    }

    public static void CenterColumnText(string text, bool underlined = false)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() * 0.5f) - (ImGui.CalcTextSize(text).X * 0.5f));
        if(underlined)
        {
            TextUnderlined(text);
        }
        else
        {
            Text(text);
        }
    }

    public static void CenterColumnText(Vector4 colour, string text, bool underlined = false)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, colour);
        CenterColumnText(text, underlined);
        ImGui.PopStyleColor();
    }

    public static void TextUnderlined(uint color, string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        TextUnderlined(text);
        ImGui.PopStyleColor();
    }

    public static void TextUnderlined(Vector4 color, string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        TextUnderlined(text);
        ImGui.PopStyleColor();
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
        ImGuiEx.Text(text);
    }
}
