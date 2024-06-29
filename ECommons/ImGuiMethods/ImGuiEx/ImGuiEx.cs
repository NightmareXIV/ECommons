using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Action = System.Action;

namespace ECommons.ImGuiMethods;
#nullable disable

public static unsafe partial class ImGuiEx
{
    public const ImGuiWindowFlags OverlayFlags = ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing;

    public readonly record struct RequiredPluginInfo
    {
        public readonly string InternalName;
        public readonly string? VanityName;
        public readonly Version? MinVersion;

				public RequiredPluginInfo(string internalName) : this()
				{
						InternalName = internalName;
				}

				public RequiredPluginInfo(string internalName, string vanityName) : this()
				{
						InternalName = internalName;
						VanityName = vanityName;
				}

				public RequiredPluginInfo(string internalName, Version minVersion) : this()
				{
						InternalName = internalName;
						MinVersion = minVersion;
				}

				public RequiredPluginInfo(string internalName, string vanityName, Version minVersion)
				{
						InternalName = internalName;
						VanityName = vanityName;
						MinVersion = minVersion;
				}
		}
    public static void PluginAvailabilityIndicator(IEnumerable<RequiredPluginInfo> pluginInfos, string prependText = "The following plugins are required to be installed and enabled:")
    {
				var pass = pluginInfos.All(info => Svc.PluginInterface.InstalledPlugins.Any(x => x.IsLoaded && x.InternalName == info.InternalName && (info.MinVersion == null || x.Version >= info.MinVersion)));

        ImGui.SameLine();
				ImGui.PushFont(UiBuilder.IconFont);
				ImGuiEx.Text(pass?ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, pass?FontAwesomeIcon.Check.ToIconString():"\uf00d");
				ImGui.PopFont();
				if (ImGui.IsItemHovered())
				{
						ImGui.BeginTooltip();
						ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGuiEx.Text(prependText);
						ImGui.PopTextWrapPos();
						foreach (var info in pluginInfos)
            {
                var plugin = Svc.PluginInterface.InstalledPlugins.FirstOrDefault(x => x.IsLoaded && x.InternalName == info.InternalName);
                if(plugin != null)
                {
                    if(info.MinVersion == null || plugin.Version >= info.MinVersion)
                    {
                        ImGuiEx.Text(ImGuiColors.ParsedGreen, $"- {info.VanityName ?? info.InternalName}" + (info.MinVersion == null?"":$" {info.MinVersion}+"));
                    }
                    else
                    {
												ImGuiEx.Text(ImGuiColors.ParsedGreen, $"- {info.VanityName ?? info.InternalName} ");
                        ImGui.SameLine(0, 0);
												ImGuiEx.Text(ImGuiColors.DalamudRed, $"{info.MinVersion}+ ");
												ImGui.SameLine(0, 0);
                        ImGuiEx.Text($"(outdated)");
										}
                }
                else
                {
										ImGuiEx.Text(ImGuiColors.DalamudRed, $"- {info.VanityName ?? info.InternalName} " + (info.MinVersion == null?"":$"{info.MinVersion}+ "));
										ImGui.SameLine(0, 0);
										ImGuiEx.Text($"(not installed)");
								}
            }
						ImGui.EndTooltip();
				}
				
    }

    public static bool Selectable(Vector4? color, string id)
    {
        var ret = ImGuiEx.TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Leaf);
        return ret;
    }

    public static bool Selectable(string id) => Selectable(null, id);

    public static bool Selectable(string id, ref bool selected) => Selectable(null, id, ref selected);

    public static bool Selectable(Vector4? color, string id, ref bool selected, ImGuiTreeNodeFlags extraFlags = ImGuiTreeNodeFlags.Leaf)
    {
        ImGuiEx.TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | (selected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) | extraFlags);
        var ret = ImGui.IsItemClicked(ImGuiMouseButton.Left);
        if (ret) selected = !selected;
        return ret;
    }

    public static bool TreeNode(string name, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None) => ImGuiEx.TreeNode(null, name, flags);

    public static bool TreeNode(Vector4? color, string name, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None)
    {
        flags |= ImGuiTreeNodeFlags.SpanFullWidth;
        if (color != null) ImGui.PushStyleColor(ImGuiCol.Text, color.Value);
        var ret = ImGui.TreeNodeEx(name, flags);
        if (color != null) ImGui.PopStyleColor();
        return ret;
    }

    public enum JobSelectorOption 
    { 
        None, 
        /// <summary>
        /// With this option, base jobs will be included as well.
        /// </summary>
        IncludeBase,
        /// <summary>
        /// Whether to clear filter when user opens job selection menu.
        /// </summary>
        ClearFilterOnOpen,
    }
    static string JobSelectorFilter = "";
    /// <summary>
    /// ImGui combo that opens up into a multiple job selector with icons and search field.
    /// </summary>
    /// <param name="id">Standard ID that will be passed directly to the combo.</param>
    /// <param name="selectedJobs">A collection where selected jobs will be written.</param>
    /// <param name="options">An array of extra options, if desired.</param>
    /// <param name="maxPreviewJobs">How much jobs maximum will be visible on a preview before it will just display amount.</param>
    /// <param name="noJobSelectedPreview">Preview value that should be displayed when no job is selected.</param>
    /// <param name="jobDisplayFilter">Optional extra filter for jobs to be displayed.</param>
    /// <returns><see langword="true"/> every time <paramref name="selectedJobs"/> is modified.</returns>
    public static bool JobSelector(string id, ICollection<Job> selectedJobs, JobSelectorOption[] options = null, int maxPreviewJobs = 3, string noJobSelectedPreview = "None", Func<Job, bool> jobDisplayFilter = null)
    {
        var ret = false;
        var baseJobs = options?.Contains(JobSelectorOption.IncludeBase) == true;
        string preview;
        if(selectedJobs.Count == 0)
        {
            preview = noJobSelectedPreview;
        }
        else if(selectedJobs.Count > maxPreviewJobs)
        {
            preview = $"{selectedJobs.Count} selected";
        }
        else
        {
            preview = selectedJobs.Select(x => x.ToString().Replace("_", " ")).Print();
        }
        if (ImGui.BeginCombo(id, preview))
        {
            if(ImGui.IsWindowAppearing() && options?.Contains(JobSelectorOption.ClearFilterOnOpen) == true)
            ImGui.SetNextItemWidth(150f);
            ImGui.InputTextWithHint("##filter", "Filter...", ref JobSelectorFilter, 50);
            foreach (var cond in Enum.GetValues<Job>().Where(x => baseJobs || !x.IsUpgradeable()).OrderByDescending(x => Svc.Data.GetExcelSheet<ClassJob>().GetRow((uint)x).Role))
            {
                if (cond == Job.ADV) continue;
                if (jobDisplayFilter != null && !jobDisplayFilter(cond)) continue;
                var name = cond.ToString().Replace("_", " ");
                if (JobSelectorFilter == "" || name.Contains(JobSelectorFilter, StringComparison.OrdinalIgnoreCase))
                {
                    if (ThreadLoadImageHandler.TryGetIconTextureWrap((uint)cond.GetIcon(), false, out var texture))
                    {
                        ImGui.Image(texture.ImGuiHandle, new Vector2(24f));
                        ImGui.SameLine();
                    }
                    if (ImGuiEx.CollectionCheckbox(name, cond, selectedJobs)) ret = true;
                }
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    public static void HelpMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true) => InfoMarker(helpText, color, symbolOverride, sameLine);

    public static void InfoMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true)
    {
        if(sameLine) ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(color ?? ImGuiColors.DalamudGrey3, symbolOverride ?? FontAwesomeIcon.InfoCircle.ToIconString());
        ImGui.PopFont();
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGui.TextUnformatted(helpText);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

    

    

    public static bool SliderInt(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max, format, flags);
        ActivateIfDoubleClicked();
        return ret;
    }

    public static bool SliderInt(string label, ref int v, int v_min, int v_max, string format)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max, format);
        ActivateIfDoubleClicked();
        return ret;
    }

    public static bool SliderInt(string label, ref int v, int v_min, int v_max)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max);
        ActivateIfDoubleClicked();
        return ret;
    }


    public static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max, format, flags);
        ActivateIfDoubleClicked();
        return ret;
    }

    public static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max, format);
        ActivateIfDoubleClicked();
        return ret;
    }

    public static bool SliderFloat(string label, ref float v, float v_min, float v_max)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max);
        ActivateIfDoubleClicked();
        return ret;
    }

    public static void ActivateIfDoubleClicked()
    {
        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            ImGui.SetKeyboardFocusHere(-1);
        }
    }

    public static void SetNextItemWidthScaled(float width)
    {
        ImGui.SetNextItemWidth(width.Scale());
    }

    public static bool InputTextMultilineExpanding(string id, ref string text, uint maxLength = 500, int minLines = 2, int maxLines = 10, int? width = null)
    {
        return ImGui.InputTextMultiline(id, ref text, maxLength, new(width ?? ImGui.GetContentRegionAvail().X, ImGui.CalcTextSize("A").Y * Math.Clamp(text.Split("\n").Length + 1, minLines, maxLines)));
    }

    public static bool EnumOrderer<T>(string id, List<T> order) where T : IConvertible
    {
        var ret = false;
        var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        foreach (var x in enumValues) if (!order.Contains(x)) order.Add(x);
        if(order.Count > enumValues.Length)
        {
            PluginLog.Warning($"EnumOrderer: duplicates or non-existing items found, enum {enumValues.Print()}, list {order.Print()}, cleaning up.");
            order.RemoveAll(x => !enumValues.Contains(x));
            var set = order.ToHashSet();
            order.Clear();
            order.AddRange(set);
        }
        for (int i = 0; i < order.Count; i++)
        {
            var e = order[i];
            ImGui.PushID($"ECommonsEnumOrderer{id}{e}");
            if (ImGui.ArrowButton("up", ImGuiDir.Up) && i > 0)
            {
                (order[i - 1], order[i]) = (order[i], order[i - 1]);
                ret = true;
            }
            ImGui.SameLine();
            if (ImGui.ArrowButton("down", ImGuiDir.Down) && i < order.Count - 1)
            {
                (order[i + 1], order[i]) = (order[i], order[i + 1]);
                ret = true;
            }
            ImGui.SameLine();
            ImGuiEx.Text($"{e.ToString().Replace("_", " ")}");
            ImGui.PopID();
        }
        return ret;
    }

    public static bool HoveredAndClicked(string tooltip = null, ImGuiMouseButton btn = ImGuiMouseButton.Left, bool requireCtrl = false)
    {
        if (ImGui.IsItemHovered())
        {
            if (tooltip != null)
            {
                SetTooltip(tooltip);
            }
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            return (!requireCtrl || ImGui.GetIO().KeyCtrl) && ImGui.IsItemClicked(btn);
        }
        return false;
    }

    public static bool ButtonCond(string name, Func<bool> condition)
    {
        var dis = !condition();
        if (dis) ImGui.BeginDisabled();
        var ret = ImGui.Button(name);
        if (dis) ImGui.EndDisabled();
        return ret;
    }

    public static bool InputLong(string id, ref long num)
    {
        var txt = num.ToString();
        var ret = ImGui.InputText(id, ref txt, 50);
        long.TryParse(txt, out num);
        return ret;
    }

    public static bool CollapsingHeader(string text, Vector4? col = null)
    {
        if (col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        var ret = ImGui.CollapsingHeader(text);
        if (col != null) ImGui.PopStyleColor();
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
        if (col == true)
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
        if (smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if (value == null || value == false)
            {
                value = true;
            }
            else
            {
                value = false;
            }
            ret = true;
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if (value == null || value == true)
            {
                value = false;
            }
            else
            {
                value = true;
            }
            ret = true;
        }
        if (col != null) ImGui.PopStyleColor(3);
        return ret;
    }
    
    /// <summary>
    /// Converts RGB color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBB</param>
    /// <param name="alpha">Optional transparency value between 0 and 1</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGB(this uint col, float alpha = 1.0f)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f, alpha);
    }


    /// <summary>
    /// Converts RGBA color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBBAA</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGBA(this uint col)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[3] / 255f, (float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f);
    }

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, bool smallButton = false) => ButtonCheckbox(name, ref value, EColor.Red, smallButton);

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="color">Active button color</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, uint color, bool smallButton = false) => ButtonCheckbox(name, ref value, color.ToVector4(), smallButton);

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
        if (col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if (smallButton?ImGui.SmallButton(name):ImGui.Button(name))
        {
            value = !value;
            ret = true;
        }
        if (col) ImGui.PopStyleColor(3);
        return ret;
    }

    public static bool CollectionButtonCheckbox<T>(string name, T value, HashSet<T> collection, bool smallButton = false) => CollectionButtonCheckbox(name, value, collection, EColor.Red, smallButton);

    public static bool CollectionButtonCheckbox<T>(string name, T value, HashSet<T> collection, Vector4 color, bool smallButton = false)
    {
        var col = collection.Contains(value);
        var ret = false;
        if (col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if (smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if (col)
            {
                collection.Remove(value);
            }
            else
            {
                collection.Add(value);
            }
            ret = true;
        }
        if (col) ImGui.PopStyleColor(3);
        return ret;
    }

    /// <summary>
    /// Draws two radio buttons for a boolean value.
    /// </summary>
    /// <param name="labelTrue">True choice radio button text</param>
    /// <param name="labelFalse">False choice radio button text</param>
    /// <param name="value">Value</param>
    /// <param name="sameLine">Whether to draw radio buttons on the same line</param>
    /// <param name="prefix">Will be invoked before each radio button draw</param>
    /// <param name="suffix">Will be invoked after each radio button draw</param>
    public static void RadioButtonBool(string labelTrue, string labelFalse, ref bool value, bool sameLine = false, Action prefix = null, Action suffix = null)
    {
        prefix?.Invoke();
        if (ImGui.RadioButton(labelTrue, value)) value = true;
        suffix?.Invoke();
        if (sameLine) ImGui.SameLine();
        prefix?.Invoke();
        if (ImGui.RadioButton(labelFalse, !value)) value = false;
        suffix?.Invoke();
    }

    /// <summary>
    /// Draws equally sized columns without ability to resize
    /// </summary>
    /// <param name="id">Unique ImGui ID</param>
    /// <param name="values">List of actions for each column</param>
    /// <param name="columns">Force number of columns</param>
    /// <param name="extraFlags">Add extra flags to the table</param>
    public static void EzTableColumns(string id, Action[] values, int? columns = null, ImGuiTableFlags extraFlags = ImGuiTableFlags.None)
    {
        if (values.Length == 1)
        {
            GenericHelpers.Safe(values[0]);
        }
        else
        {
            if (ImGui.BeginTable(id, Math.Max(1, columns ?? values.Length), ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.NoSavedSettings | extraFlags))
            {
                foreach (Action action in values)
                {
                    ImGui.TableNextColumn();
                    GenericHelpers.Safe(action);
                }
                ImGui.EndTable();
            }
        }
    }

    public static bool ButtonCtrl(string text, string affix = " (Hold CTRL)") => ButtonCtrl(text, null, affix);

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
        if (disabled)
        {
            ImGui.BeginDisabled();
        }
        var name = string.Empty;
        if (text.Contains($"###"))
        {
            var p = text.Split($"###");
            name = $"{p[0]}{affix}###{p[1]}";
        }
        else if (text.Contains($"##"))
        {
            var p = text.Split($"##");
            name = $"{p[0]}{affix}##{p[1]}";
        }
        else
        {
            name = $"{text}{affix}";
        }
        var ret = size == null?ImGui.Button(name):ImGui.Button(name, size.Value);
        if (disabled)
        {
            ImGui.EndDisabled();
        }
        return ret;
    }

    public static bool BeginPopupNextToElement(string popupId)
    {
        ImGui.SameLine(0, 0);
        var pos = ImGui.GetCursorScreenPos();
        ImGui.Dummy(Vector2.Zero);
        ImGui.SetNextWindowPos(pos, ImGuiCond.Appearing);
        return ImGui.BeginPopup(popupId);
    }

    public record HeaderIconOptions
    {
        public Vector2 Offset { get; init; } = Vector2.Zero;
        public ImGuiMouseButton MouseButton { get; init; } = ImGuiMouseButton.Left;
        public string Tooltip { get; init; } = string.Empty;
        public uint Color { get; init; } = 0xFFFFFFFF;
        public bool ToastTooltipOnClick { get; init; } = false;
        public ImGuiMouseButton ToastTooltipOnClickButton { get; init; } = ImGuiMouseButton.Left;
    }

    private static uint headerLastWindowID = 0;
    private static ulong headerLastFrame = 0;
    private static float headerCurrentPos = 0;
    private static float headerImGuiButtonWidth = 0;

    public static bool AddHeaderIcon(string id, FontAwesomeIcon icon, HeaderIconOptions options = null)
    {
        if (ImGui.IsWindowCollapsed()) return false;

        var scale = ImGuiHelpers.GlobalScale;
        var currentID = ImGui.GetID(0);
        if (currentID != headerLastWindowID || headerLastFrame != Svc.PluginInterface.UiBuilder.FrameCount)
        {
            headerLastWindowID = currentID;
            headerLastFrame = Svc.PluginInterface.UiBuilder.FrameCount;
            headerCurrentPos = 0.25f * ImGui.GetStyle().FramePadding.Length();
            if (!GetCurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoTitleBar))
                headerCurrentPos = 1;
            headerImGuiButtonWidth = 0f;
            if (CurrentWindowHasCloseButton())
                headerImGuiButtonWidth += 17 * scale;
            if (!GetCurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoCollapse))
                headerImGuiButtonWidth += 17 * scale;
        }

        options ??= new();
        var prevCursorPos = ImGui.GetCursorPos();
        var buttonSize = new Vector2(20 * scale);
        var buttonPos = new Vector2((ImGui.GetWindowWidth() - buttonSize.X - headerImGuiButtonWidth * scale * headerCurrentPos) - (ImGui.GetStyle().FramePadding.X * scale), ImGui.GetScrollY() + 1);
        ImGui.SetCursorPos(buttonPos);
        var drawList = ImGui.GetWindowDrawList();
        drawList.PushClipRectFullScreen();

        var pressed = false;
        ImGui.InvisibleButton(id, buttonSize);
        var itemMin = ImGui.GetItemRectMin();
        var itemMax = ImGui.GetItemRectMax();
        var halfSize = ImGui.GetItemRectSize() / 2;
        var center = itemMin + halfSize;
        if (ImGui.IsWindowHovered() && ImGui.IsMouseHoveringRect(itemMin, itemMax, false))
        {
            if (!string.IsNullOrEmpty(options.Tooltip))
                ImGui.SetTooltip(options.Tooltip);
            ImGui.GetWindowDrawList().AddCircleFilled(center, halfSize.X, ImGui.GetColorU32(ImGui.IsMouseDown(ImGuiMouseButton.Left) ? ImGuiCol.ButtonActive : ImGuiCol.ButtonHovered));
            if (ImGui.IsMouseReleased(options.MouseButton))
                pressed = true;
#pragma warning disable
            if (options.ToastTooltipOnClick && ImGui.IsMouseReleased(options.ToastTooltipOnClickButton))
                Notify.Info(options.Tooltip!);
#pragma warning restore
        }

        ImGui.SetCursorPos(buttonPos);
        ImGui.PushFont(UiBuilder.IconFont);
        var iconString = icon.ToIconString();
        drawList.AddText(UiBuilder.IconFont, ImGui.GetFontSize(), itemMin + halfSize - ImGui.CalcTextSize(iconString) / 2 + options.Offset, options.Color, iconString);
        ImGui.PopFont();

        ImGui.PopClipRect();
        ImGui.SetCursorPos(prevCursorPos);

        return pressed;
    }


    public static Vector4 MutateColor(ImGuiCol col, byte r, byte g, byte b)
    {
        return ImGui.GetStyle().Colors[(int)col] with { X = (float)r / 255f, Y = (float)g / 255f, Z = (float)b / 255f };
    }

    /// <summary>
    /// Displays ImGui.SliderFloat for internal int value.
    /// </summary>
    /// <param name="id">ImGui ID</param>
    /// <param name="value">Integer value</param>
    /// <param name="min">Minimal value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="divider">Value is divided by divider before being presented to user</param>
    /// <returns></returns>
    public static bool SliderIntAsFloat(string id, ref int value, int min, int max, float divider = 1000)
    {
        var f = (float)value / divider;
        var ret = ImGui.SliderFloat(id, ref f, (float)min / divider, (float)max / divider);
        if (ret)
        {
            value = (int)(f * divider);
        }
        return ret;
    }

    public static bool IsKeyPressed(int key, bool repeat)
    {
        byte repeat2 = (byte)(repeat ? 1 : 0);
        return ImGuiNative.igIsKeyPressed((ImGuiKey)key, repeat2) != 0;
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

    public static float GetWindowContentRegionWidth()
    {
        return ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    }

    public static void Spacing(float pix = 10f, bool accountForScale = true)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (accountForScale ? pix : pix * ImGuiHelpers.GlobalScale));
    }

    public static float Scale(this float f)
    {
        // Dalamud global scale and font size are now indepedent from each other, so both need to factored in.
        return f * ImGuiHelpers.GlobalScale * (Svc.PluginInterface.UiBuilder.DefaultFontSpec.SizePt / 12f);
    }

    public static void SetTooltip(string text)
    {
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
        ImGui.TextUnformatted(text);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }

    public static void SetNextItemFullWidth(int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X + mod);
    }

    public static void SetNextItemWidth(float percent, int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X * percent + mod);
    }

    static Dictionary<string, Box<string>> InputListValuesString = new();
    public static void InputListString(string name, List<string> list, Dictionary<string, string> overrideValues = null)
    {
        if (!InputListValuesString.ContainsKey(name)) InputListValuesString[name] = new("");
        InputList(name, list, overrideValues, delegate
        {
            var buttonSize = ImGuiHelpers.GetButtonSize("Add");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
            ImGui.InputText($"##{name.Replace("#", "_")}", ref InputListValuesString[name].Value, 100);
            ImGui.SameLine();
            if (ImGui.Button("Add"))
            {
                list.Add(InputListValuesString[name].Value);
                InputListValuesString[name].Value = "";
            }
        });
    }

    static Dictionary<string, Box<uint>> InputListValuesUint = new();
    public static void InputListUint(string name, List<uint> list, Dictionary<uint, string> overrideValues = null)
    {
        if (!InputListValuesUint.ContainsKey(name)) InputListValuesUint[name] = new(0);
        InputList(name, list, overrideValues, delegate
        {
            var buttonSize = ImGuiHelpers.GetButtonSize("Add");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
            ImGuiEx.InputUint($"##{name.Replace("#", "_")}", ref InputListValuesUint[name].Value);
            ImGui.SameLine();
            if (ImGui.Button("Add"))
            {
                list.Add(InputListValuesUint[name].Value);
                InputListValuesUint[name].Value = 0;
            }
        });
    }

    public static void InputList<T>(string name, List<T> list, Dictionary<T, string> overrideValues, Action addFunction)
    {
        var text = list.Count == 0 ? "- No values -" : (list.Count == 1 ? $"{(overrideValues != null && overrideValues.ContainsKey(list[0]) ? overrideValues[list[0]] : list[0])}" : $"- {list.Count} elements -");
        if (ImGui.BeginCombo(name, text))
        {
            addFunction();
            var rem = -1;
            for (var i = 0; i < list.Count; i++)
            {
                var id = $"{name}ECommonsDeleItem{i}";
                var x = list[i];
                ImGui.Selectable($"{(overrideValues != null && overrideValues.ContainsKey(x) ? overrideValues[x] : x)}");
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup(id);
                }
                if (ImGui.BeginPopup(id))
                {
                    if (ImGui.Selectable("Delete##ECommonsDeleItem"))
                    {
                        rem = i;
                    }
                    if (ImGui.Selectable("Clear (hold shift+ctrl)##ECommonsDeleItem")
                        && ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl)
                    {
                        rem = -2;
                    }
                    ImGui.EndPopup();
                }
            }
            if (rem > -1)
            {
                list.RemoveAt(rem);
            }
            if (rem == -2)
            {
                list.Clear();
            }
            ImGui.EndCombo();
        }
    }

    public static void WithTextColor(Vector4 col, Action func)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        GenericHelpers.Safe(func);
        ImGui.PopStyleColor();
    }

    public static void Tooltip(string s)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            SetTooltip(s);
            ImGui.PopTextWrapPos();
        }
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
        var cur = ImGui.GetCursorPos();
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button("");
        ImGui.PopStyleVar();
        ImGui.SameLine();
        ImGui.SetCursorPos(cur);
        ImGui.TextUnformatted(s);
    }

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

    public static void Text(Vector4 col, ImFontPtr font, string s)
    {
        ImGui.PushFont(font);
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
        ImGui.PopFont();
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

    public static Vector4 GetParsedColor(int percent)
    {
        if (percent < 25)
        {
            return ImGuiColors.ParsedGrey;
        }
        else if (percent < 50)
        {
            return ImGuiColors.ParsedGreen;
        }
        else if (percent < 75)
        {
            return ImGuiColors.ParsedBlue;
        }
        else if (percent < 95)
        {
            return ImGuiColors.ParsedPurple;
        }
        else if (percent < 99)
        {
            return ImGuiColors.ParsedOrange;
        }
        else if (percent == 99)
        {
            return ImGuiColors.ParsedPink;
        }
        else if (percent == 100)
        {
            return ImGuiColors.ParsedGold;
        }
        else
        {
            return ImGuiColors.DalamudRed;
        }
    }

    public static void EzTabBar(string id, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, openTabName, ImGuiTabBarFlags.None, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, ImGuiTabBarFlags flags, params (string name, Action function, Vector4? color, bool child)[] tabs)
    {
        ImGui.BeginTabBar(id, flags);
        foreach (var x in tabs)
        {
            if (x.name == null) continue;
            if (x.color != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, x.color.Value);
            }
            if (ImGuiEx.BeginTabItem(x.name, openTabName == x.name?ImGuiTabItemFlags.SetSelected:ImGuiTabItemFlags.None))
            {
                if (x.color != null)
                {
                    ImGui.PopStyleColor();
                }
                if (x.child) ImGui.BeginChild(x.name + "child");
                x.function();
                if (x.child) ImGui.EndChild();
                ImGui.EndTabItem();
            }
            else
            {
                if (x.color != null)
                {
                    ImGui.PopStyleColor();
                }
            }
        }
        if (KoFiTransparent != null) KoFiButton.RightTransparentTab(KoFiTransparent);
        ImGui.EndTabBar();
    }

    public static void InvisibleButton(int width = 0)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button(" ");
        ImGui.PopStyleVar();
    }

    public static Dictionary<string, Box<string>> EnumComboSearch = new();
    /// <summary>
    /// Draws an easy combo selector for an enum with a search field for long lists.
    /// </summary>
    /// <typeparam name="T">Enum</typeparam>
    /// <param name="name">ImGui ID</param>
    /// <param name="refConfigField">Value</param>
    /// <param name="names">Optional Name overrides</param>
    public static bool EnumCombo<T>(string name, ref T refConfigField, IDictionary<T, string> names) where T : Enum, IConvertible
    {
        return EnumCombo(name, ref refConfigField, null, names);
    }

    /// <summary>
    /// Draws an easy combo selector for an enum with a search field for long lists.
    /// </summary>
    /// <typeparam name="T">Enum</typeparam>
    /// <param name="name">ImGui ID</param>
    /// <param name="refConfigField">Value</param>
    /// <param name="filter">Optional filter</param>
    /// <param name="names">Optional Name overrides</param>
    /// <returns></returns>
    public static bool EnumCombo<T>(string name, ref T refConfigField, Func<T, bool> filter = null, IDictionary<T, string> names = null) where T : Enum, IConvertible
    {
        var ret = false;
        if (ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString().Replace("_", " ")))
        {
            var values = Enum.GetValues(typeof(T));
            Box<string> fltr = null;
            if (values.Length > 10)
            {
                if (!EnumComboSearch.ContainsKey(name)) EnumComboSearch.Add(name, new(""));
                fltr = EnumComboSearch[name];
                ImGuiEx.SetNextItemFullWidth();
                ImGui.InputTextWithHint($"##{name.Replace("#", "_")}", "Filter...", ref fltr.Value, 50);
            }
            foreach (var x in values)
            {
                var equals = EqualityComparer<T>.Default.Equals((T)x, refConfigField);
                var element = (names != null && names.TryGetValue((T)x, out n)) ? n : x.ToString().Replace("_", " ");
                if ((filter == null || filter((T)x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Selectable(element, equals)
                    )
                {
                    ret = true;
                    refConfigField = (T)x;
                }
                if (ImGui.IsWindowAppearing() && equals) ImGui.SetScrollHereY();
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    public static bool EnumRadio<T>(ref T refConfigField, bool sameLine = false, Func<T, bool> filter = null, IDictionary<T, string> names = null) where T : Enum, IConvertible
    {
        var ret = false;
        var values = Enum.GetValues(typeof(T));
        bool first = true;
        foreach (var x in values)
        {
            if (!first && sameLine) ImGui.SameLine();
            first = false;
            var equals = EqualityComparer<T>.Default.Equals((T)x, refConfigField);
            var element = (names != null && names.TryGetValue((T)x, out var n)) ? n : x.ToString().Replace("_", " ");
            if ((filter == null || filter((T)x))
                && ImGui.RadioButton(element, equals)
                )
            {
                ret = true;
                refConfigField = (T)x;
            }
        }
        return ret;
    }

    public static Dictionary<string, Box<string>> ComboSearch = new();
    public static bool Combo<T>(string name, ref T refConfigField, IEnumerable<T> values, Func<T, bool> filter = null, Dictionary<T, string> names = null)
    {
        var ret = false;
        if (ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString()))
        {
            Box<string> fltr = null;
            if (values.Count() > 10)
            {
                if (!ComboSearch.ContainsKey(name)) ComboSearch.Add(name, new(""));
                fltr = ComboSearch[name];
                ImGuiEx.SetNextItemFullWidth();
                ImGui.InputTextWithHint($"##{name}fltr", "Filter...", ref fltr.Value, 50);
            }
            foreach (var x in values)
            {
                var equals = EqualityComparer<T>.Default.Equals(x, refConfigField);
                var element = (names != null && names.TryGetValue(x, out n)) ? n : x.ToString();
                if ((filter == null || filter(x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Selectable(element, equals)
                    )
                {
                    ret = true;
                    refConfigField = x;
                }
                if (ImGui.IsWindowAppearing() && equals) ImGui.SetScrollHereY();
            }
            ImGui.EndCombo();
        }
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

    public static bool Ctrl => ImGui.GetIO().KeyCtrl;
    public static bool Alt => ImGui.GetIO().KeyAlt;
    public static bool Shift => ImGui.GetIO().KeyShift;

    public static bool IconButton(FontAwesomeIcon icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        return IconButton(icon.ToIconString(), id, size, enabled);
    }

    public static bool SmallButton(string label, bool enabled = true)
    {
        if (!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.SmallButton(label) && enabled;
        if (!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool Button(string label, bool enabled = true)
    {
        if (!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.Button(label) && enabled;
        if (!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool Button(string label, Vector2 size, bool enabled = true)
    {
        if (!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.Button(label, size) && enabled;
        if (!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool IconButton(string icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        if (!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var result = ImGui.Button($"{icon}##{icon}-{id}", size) && enabled;
        if (!enabled) ImGui.PopStyleVar();
        ImGui.PopFont();
        return result;
    }

    public static bool IconButtonWithText(FontAwesomeIcon icon, string id, bool enabled = true)
    {
        if (!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var result = ImGuiComponents.IconButtonWithText(icon, $"{id}") && enabled;
        if (!enabled) ImGui.PopStyleVar();
        return result;
    }

    public static Vector2 CalcIconSize(FontAwesomeIcon icon, bool isButton = false)
    {
        return CalcIconSize(icon.ToIconString(), isButton);
    }

    public static Vector2 CalcIconSize(string icon, bool isButton = false)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.CalcTextSize($"{icon}");
        ImGui.PopFont();
        return result + (isButton ? ImGui.GetStyle().FramePadding * 2f : Vector2.Zero);
    }

    public static float Measure(Action func, bool includeSpacing = true)
    {
        var pos = ImGui.GetCursorPosX();
        func();
        ImGui.SameLine(0, 0);
        var diff = ImGui.GetCursorPosX() - pos;
        ImGui.Dummy(Vector2.Zero);
        return diff + (includeSpacing ? ImGui.GetStyle().ItemSpacing.X : 0);
    }

    public static void InputHex(string name, ref uint hexInt)
    {
        var text = $"{hexInt:X}";
        if (ImGui.InputText(name, ref text, 50))
        {
            if (uint.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, null, out var num))
            {
                hexInt = num;
            }
        }
    }

    public static void InputHex(string name, ref long hexInt)
    {
        var text = $"{hexInt:X}";
        if (ImGui.InputText(name, ref text, 50))
        {
            if (long.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, null, out var num))
            {
                hexInt = num;
            }
        }
    }

    public static void InputHex(string name, ref byte hexByte)
    {
        var text = $"{hexByte:X}";
        if (ImGui.InputText(name, ref text, 2))
        {
            if (byte.TryParse(text, NumberStyles.HexNumber, null, out var num))
            {
                hexByte = num;
            }
        }
    }

    public static void InputUint(string name, ref uint uInt)
    {
        var text = $"{uInt}";
        if (ImGui.InputText(name, ref text, 16))
        {
            if (uint.TryParse(text, out var num))
            {
                uInt = num;
            }
        }
    }

    public static void TextCopy(Vector4 col, string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        TextCopy(text);
        ImGui.PopStyleColor();
    }

    public static void TextCopy(string text)
    {
        ImGui.TextUnformatted(text);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
#pragma warning disable
            GenericHelpers.Copy(text);
#pragma warning restore
        }
    }

    public static void TextWrappedCopy(string text)
    {
        ImGuiEx.TextWrapped(text);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
#pragma warning disable
            GenericHelpers.Copy(text);
#pragma warning restore
        }
    }

    public static void TextWrappedCopy(Vector4 col, string text)
    {
        ImGuiEx.TextWrapped(col, text);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
#pragma warning disable
            GenericHelpers.Copy(text);
#pragma warning restore
        }
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

    public static void TextCentered(Vector4? col, string text)
    {
        if (col == null) 
        {
            TextCentered(text);
        }
        else 
        {
            TextCentered(col.Value, text);
        }
    }

    public static void ButtonCopy(string buttonText, string copy)
    {
        if (ImGui.Button(buttonText.Replace("$COPY", copy)))
        {
#pragma warning disable
            GenericHelpers.Copy(copy);
#pragma warning restore
        }
    }

    public static void CenterColumnText(string text, bool underlined = false)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() * 0.5f) - (ImGui.CalcTextSize(text).X * 0.5f));
        if (underlined)
            TextUnderlined(text);
        else
            Text(text);
    }

    public static void CenterColumnText(Vector4 colour, string text, bool underlined = false)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, colour);
        CenterColumnText(text, underlined);
        ImGui.PopStyleColor();
    }

    public unsafe static bool BeginTabItem(string label, ImGuiTabItemFlags flags)
    {
        int num = 0;
        byte* ptr;
        if (label != null)
        {
            num = Encoding.UTF8.GetByteCount(label);
            ptr = Allocate(num + 1);
            int utf = GetUtf8(label, ptr, num);
            ptr[utf] = 0;
        }
        else
        {
            ptr = null;
        }

        byte* p_open2 = null;
        byte num2 = ImGuiNative.igBeginTabItem(ptr, p_open2, flags);
        if (num > 2048)
        {
            Free(ptr);
        }
        return num2 != 0;
    }

    internal unsafe static byte* Allocate(int byteCount)
    {
        return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
    }

    internal unsafe static void Free(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    internal unsafe static int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed (char* chars = s)
        {
            return Encoding.UTF8.GetBytes(chars, s.Length, utf8Bytes, utf8ByteCount);
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct ImGuiWindow
{
    [FieldOffset(0xC)] public ImGuiWindowFlags Flags;

    [FieldOffset(0xD5)] public byte HasCloseButton;

    // 0x118 is the start of ImGuiWindowTempData
    [FieldOffset(0x130)] public Vector2 CursorMaxPos;
}

public static partial class ImGuiEx
{
    [LibraryImport("cimgui")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial nint igGetCurrentWindow();
    public static unsafe ImGuiWindow* GetCurrentWindow() => (ImGuiWindow*)igGetCurrentWindow();
    public static unsafe ImGuiWindowFlags GetCurrentWindowFlags() => GetCurrentWindow()->Flags;
    public static unsafe bool CurrentWindowHasCloseButton() => GetCurrentWindow()->HasCloseButton != 0;

}

