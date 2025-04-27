using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;
public static partial class ImGuiEx
{
    public static bool Button(string label, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var ret = ImGui.Button(label) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool Button(string label, Vector2 size, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var ret = ImGui.Button(label, size) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool ButtonScaled(string label, Vector2 size, bool enabled = true)
    {
        return Button(label, size.Scale(), enabled);
    }

    public static bool IconButton(FontAwesomeIcon icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        return IconButton(icon.ToIconString(), id, size, enabled);
    }

    public static bool IconButton(string icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var result = ImGui.Button($"{icon}##{icon}-{id}", size) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        ImGui.PopFont();
        return result;
    }

    public static bool IconButtonScaled(FontAwesomeIcon icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        return IconButton(icon, id, size.Scale(), enabled);
    }

    public static bool IconButtonScaled(string icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        return IconButton(icon, id, size.Scale(), enabled);
    }

    public static bool IconButtonWithText(FontAwesomeIcon icon, string id, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var result = ImGuiComponents.IconButtonWithText(icon, $"{id}") && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return result;
    }

    public static bool SmallButton(string label, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var ret = ImGui.SmallButton(label) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool SmallIconButton(FontAwesomeIcon icon, string id = "ECommonsButton")
    {
        return SmallIconButton(icon.ToIconString(), id);
    }

    public static bool SmallIconButton(string icon, string id = "ECommonsButton")
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.SmallButton($"{icon}##{icon}-{id}");
        ImGui.PopFont();
        return result;
    }

    /// <summary>
    /// Draws a button that will copy text to clipboard when clicked.
    /// </summary>
    /// <remarks>Will also replace any <c>$COPY</c> in the button text with the text to copy</remarks>
    public static bool ButtonCopy(string buttonText, string copy)
    {
        if(ImGui.Button(buttonText.Replace("$COPY", copy)))
        {
            GenericHelpers.Copy(copy);
            return true;
        }
        return false;
    }

    public static void InvisibleButton(int width = 0)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button(" ");
        ImGui.PopStyleVar();
    }

    /// <summary>
    /// Draws a button that will be on the same line as previous if there is space, otherwise will move to the next line.
    /// </summary>
    /// <param name="label">Button label</param>
    /// <remarks><see cref="ImGui.SameLine()"/> does not need to be called just before using this.</remarks>
    /// <returns></returns>
    public static bool ButtonWrapped(string label)
    {
        ImGui.SameLine();
        var labelW = ImGuiHelpers.GetButtonSize(label);
        var finishPos = ImGui.GetCursorPosX() + labelW.X;
        if(finishPos >= ImGui.GetContentRegionMax().X)
            ImGui.NewLine();

        return ImGui.Button(label);
    }

    public static bool ButtonCtrl(string text, string affix = " (Hold CTRL)")
    {
        return ButtonCtrl(text, null, affix);
    }

    /// <summary>
    /// Button that is disabled unless CTRL key is held
    /// </summary>
    /// <param name="text">Button ID</param>
    /// <param name="size">Optional size of the button, null if size is to be calculated automatically</param>
    /// <param name="affix">Button affix</param>
    /// <returns></returns>
    public static bool ButtonCtrl(string text, Vector2? size, string affix = " (Hold CTRL)")
    {
        var disabled = !ImGui.GetIO().KeyCtrl;
        if(disabled)
        {
            ImGui.BeginDisabled();
        }
        var name = string.Empty;
        if(text.Contains($"###"))
        {
            var p = text.Split($"###");
            name = $"{p[0]}{affix}###{p[1]}";
        }
        else if(text.Contains($"##"))
        {
            var p = text.Split($"##");
            name = $"{p[0]}{affix}##{p[1]}";
        }
        else
        {
            name = $"{text}{affix}";
        }
        var ret = size == null ? ImGui.Button(name) : ImGui.Button(name, size.Value);
        if(disabled)
        {
            ImGui.EndDisabled();
        }
        return ret;
    }

    public static bool ButtonCtrlScaled(string text, Vector2? size, string affix = " (Hold CTRL)")
    {
        return ButtonCtrl(text, size.Scale(), affix);
    }

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, bool smallButton)
    {
        return ButtonCheckbox(name, ref value, EColor.Red, smallButton);
    }

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="color">Active button color</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, uint color, bool smallButton = false)
    {
        return ButtonCheckbox(name, ref value, color.ToVector4(), smallButton);
    }

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="color">Active button color</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, Vector4 color, bool smallButton = false)
    {
        var col = value;
        var ret = false;
        if(col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if(smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            value = !value;
            ret = true;
        }
        if(col) ImGui.PopStyleColor(3);
        return ret;
    }

    public static bool ButtonCheckbox(FontAwesomeIcon icon, ref bool value, Vector4? color = null, bool inverted = false)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var ret = ButtonCheckbox(icon.ToIconString(), ref value, color, inverted);
        ImGui.PopFont();
        return ret;
    }

    public static bool ButtonCheckbox(string name, ref bool value, Vector4? color = null, bool inverted = false)
    {
        var ret = false;
        color ??= EColor.Green;
        var col = !inverted ? value : !value;
        if(col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);
        }
        if(ImGui.Button(name))
        {
            value = !value;
            ret = true;
        }
        if(col) ImGui.PopStyleColor(3);
        return ret;
    }

    /// <summary>
    /// Provides a button that can be used to switch <see langword="bool"/>? variables. Left click - to toggle between <see langword="true"/> and <see langword="null"/>, right click - to toggle between <see langword="false"/> and <see langword="null"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="TrueColor">Color when <paramref name="value"/> is true</param>
    /// <param name="FalseColor">Color when <paramref name="value"/> is false</param>
    /// <param name="smallButton">Whether a button should be small</param>
    /// <returns></returns>
    public static bool ButtonCheckbox(string name, ref bool? value, Vector4? TrueColor = null, Vector4? FalseColor = null, bool smallButton = false)
    {
        TrueColor ??= EColor.Green;
        FalseColor ??= EColor.Red;
        var col = value;
        var ret = false;
        if(col == true)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, TrueColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, TrueColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, TrueColor.Value);
        }
        else if(col == false)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, FalseColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, FalseColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, FalseColor.Value);
        }
        if(smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if(value == null || value == false)
            {
                value = true;
            }
            else
            {
                value = false;
            }
            ret = true;
        }
        if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if(value == null || value == true)
            {
                value = false;
            }
            else
            {
                value = true;
            }
            ret = true;
        }
        if(col != null) ImGui.PopStyleColor(3);
        return ret;
    }

    [Obsolete($"Use {nameof(Button)}")]
    public static bool ButtonCond(string name, Func<bool> condition)
    {
        var dis = !condition();
        if(dis) ImGui.BeginDisabled();
        var ret = ImGui.Button(name);
        if(dis) ImGui.EndDisabled();
        return ret;
    }
}
