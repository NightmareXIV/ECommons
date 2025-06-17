using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ECommons.MathHelpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
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

    public static bool ButtonCheckbox(FontAwesomeIcon icon, ref bool value, Vector4? color = null, bool inverted = false, Vector2? size = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var ret = ButtonCheckbox(icon.ToIconString(), ref value, color, inverted, size);
        ImGui.PopFont();
        return ret;
    }

    public static bool ButtonCheckbox(string name, ref bool value, Vector4? color = null, bool inverted = false, Vector2? size = null)
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
        if((size == null) ? ImGui.Button(name) : ImGui.Button(name, size.Value))
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

    /// <summary>
    /// Better version of Dalamud's <see cref="ImGuiHelpers.HorizontalButtonGroup"/>
    /// </summary>
    public class EzButtonGroup
    {
        private readonly List<(string Label, Action Action, ButtonStyle Style)> buttons = [];
        private readonly List<(FontAwesomeIcon Icon, string Label, Action Action, ButtonStyle Style)> iconWithTextButtons = [];
        private readonly List<(FontAwesomeIcon Icon, Action Action, ButtonStyle Style)> iconOnlyButtons = [];
        public bool IsCentered { get; set; }
        public float Height { get; set; } = ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2;
        public float Width { get; private set; }

        public class ButtonStyle
        {
            public bool? NoButtonBg { get; set; }
            public Vector4? ButtonColor { get; set; }
            public Vector4? TextColor { get; set; }
            public string? Tooltip { get; set; }
            public Func<bool>? Condition { get; set; }

            public static ButtonStyle operator +(ButtonStyle? a, ButtonStyle? b)
            {
                if(a == null && b == null) return new ButtonStyle();
                if(a == null) return b!;
                if(b == null) return a;

                return new ButtonStyle
                {
                    NoButtonBg = b.NoButtonBg ?? a.NoButtonBg,
                    ButtonColor = b.ButtonColor ?? a.ButtonColor,
                    TextColor = b.TextColor ?? a.TextColor,
                    Tooltip = b.Tooltip ?? a.Tooltip,
                    Condition = b.Condition ?? a.Condition
                };
            }
        }

        public void Add(string label, Action action, ButtonStyle? style = null) => buttons.Add((label, action, style));
        public void Add(string label, Action action, string tooltip, ButtonStyle? style = null) => buttons.Add((label, action, style + new ButtonStyle { Tooltip = tooltip }));
        public void AddIconWithText(FontAwesomeIcon icon, string label, Action action, ButtonStyle? style = null) => iconWithTextButtons.Add((icon, label, action, style));
        public void AddIconWithText(FontAwesomeIcon icon, string label, Action action, string tooltip, ButtonStyle? style = null) => iconWithTextButtons.Add((icon, label, action, style + new ButtonStyle { Tooltip = tooltip }));
        public void AddIconOnly(FontAwesomeIcon icon, Action action, ButtonStyle? style = null) => iconOnlyButtons.Add((icon, action, style));
        public void AddIconOnly(FontAwesomeIcon icon, Action action, string tooltip, ButtonStyle? style = null) => iconOnlyButtons.Add((icon, action, style + new ButtonStyle { Tooltip = tooltip }));

        public void Draw()
        {
            var buttonSizes = new List<Vector2>();

            // Calculate total width and store button sizes
            foreach(var (label, _, _) in buttons)
            {
                var size = ImGuiHelpers.GetButtonSize(label);
                buttonSizes.Add(size);
                Width += size.X + ImGui.GetStyle().ItemSpacing.X;
            }

            foreach(var (icon, label, _, _) in iconWithTextButtons)
            {
                var size = ImGuiHelpers.GetButtonSize(label);
                size.X += ImGui.CalcTextSize(icon.ToIconString()).X + ImGui.GetStyle().ItemSpacing.X;
                buttonSizes.Add(size);
                Width += size.X + ImGui.GetStyle().ItemSpacing.X;
            }

            foreach(var (icon, _, _) in iconOnlyButtons)
            {
                var size = ImGui.CalcTextSize(icon.ToIconString());
                buttonSizes.Add(size);
                Width += size.X + ImGui.GetStyle().ItemSpacing.X;
            }

            // Remove last spacing
            Width -= ImGui.GetStyle().ItemSpacing.X;

            if(IsCentered)
            {
                var windowWidth = ImGui.GetContentRegionAvail().X;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (windowWidth - Width) * 0.5f);
            }

            // Draw buttons
            var buttonIndex = 0;
            foreach(var (label, action, style) in buttons)
            {
                if(buttonIndex > 0)
                    ImGui.SameLine();

                DrawButton(style, () =>
                {
                    if(ImGui.Button(label, new Vector2(buttonSizes[buttonIndex].X, Height)))
                        action();
                });

                buttonIndex++;
            }

            foreach(var (icon, label, action, style) in iconWithTextButtons)
            {
                if(buttonIndex > 0)
                    ImGui.SameLine();

                DrawButton(style, () =>
                {
                    if(ImGuiComponents.IconButtonWithText(icon, label))
                        action();
                });

                buttonIndex++;
            }

            foreach(var (icon, action, style) in iconOnlyButtons)
            {
                if(buttonIndex > 0)
                    ImGui.SameLine();

                DrawButton(style, () =>
                {
                    if(IconButton(icon))
                        action();
                });

                buttonIndex++;
            }
        }

        private void DrawButton(ButtonStyle? style, Action drawButton)
        {
            if(style?.Condition is { } cond && !cond()) return;

            if(style?.NoButtonBg == true)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);
            }
            else
            {
                var bCol = style?.ButtonColor ?? ImGui.GetStyle().Colors[(int)ImGuiCol.Button];
                ImGui.PushStyleColor(ImGuiCol.Button, bCol);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, bCol.AddNoW(0.1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, bCol.AddNoW(0.2f));
            }

            if(style?.TextColor is { } tCol) ImGui.PushStyleColor(ImGuiCol.Text, tCol);
            drawButton();
            if(style?.TextColor is { }) ImGui.PopStyleColor();

            ImGui.PopStyleColor(3);

            if(style?.Tooltip is { })
                Tooltip(style.Tooltip);
        }
    }
}
