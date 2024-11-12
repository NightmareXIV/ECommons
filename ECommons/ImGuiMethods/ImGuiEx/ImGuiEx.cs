using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.Funding;
using ECommons.Logging;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.Sheets;
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
    private static Dictionary<string, int> SelectedPages = [];

    public static string ImGuiTrim(this string str)
    {
        if(str.Length < 5) return str;
        var size = ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("...").X;
        for (int i = 1; i < str.Length; i++)
        {
            if(ImGui.CalcTextSize(str[..i]).X > size)
            {
                return str[..(i - 1)] + "...";
            }
        }
        return str;
    }

    public static string Trim(this string text, int len)
    {
        if(text.Length > len) return text[..len] + "...";
        return text;
    }

    /// <summary>
    /// An <see cref="ImGui.InputInt"/> for nullable int. Consists of checkbox and input component that is enabled/disabled based on checkbox state.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="label"></param>
    /// <param name="valueNullable"></param>
    /// <param name="step"></param>
    /// <param name="step_fast"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool InputInt(float width, string label, ref int? valueNullable, int step = 1, int step_fast = 100, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.PushID($"NullableInputInt{label}");
        var enabled = valueNullable != null;
        var chk = ImGui.Checkbox($"##checkbox", ref enabled);
        if(chk)
        {
            valueNullable = enabled?0:null;
        }
        ImGui.PopID();
        var value = valueNullable ?? 0;
        if(!enabled) ImGui.BeginDisabled();
        ImGui.SameLine();
        ImGui.SetNextItemWidth(width);
        var ret = ImGui.InputInt(label, ref value, step, step_fast, flags);
        if(ret) valueNullable = value;
        if(!enabled) ImGui.EndDisabled();
        return ret || chk;
    }

    /// <inheritdoc cref="Pagination(string, System.Action[], out System.Action?, int, int)"/>
    public static Action[] Pagination(Action[] actions, int perPage = 0, int maxPages = 0) => Pagination(GenericHelpers.GetCallStackID(), actions, perPage, maxPages);

    /// <inheritdoc cref="Pagination(string, System.Action[], out System.Action?, int, int)"/>
    public static Action[] Pagination(string id, Action[] actions, int perPage = 0, int maxPages = 0)
    {
        var ret = Pagination(id, actions, out var paginator, perPage, maxPages);
        paginator?.Invoke();
        return ret;
    }

    /// <inheritdoc cref="Pagination(string, System.Action[], out System.Action?, int, int)"/>
    public static Action[] Pagination(Action[] actions, out Action? paginator, int perPage = 0, int maxPages = 0) => Pagination(GenericHelpers.GetCallStackID(), actions, out paginator, perPage, maxPages);

    /// <summary>
    /// Splits array of draw actions into few pages.
    /// </summary>
    /// <param name="id">Unique ID of your paginator. Must be unique on plugin level.</param>
    /// <param name="actions">Array of actions to paginate</param>
    /// <param name="paginator">Page switcher that you have to draw. May be absent if there's no page.</param>
    /// <param name="perPage">How much elements to display per page. If set to 0, it will be automatically calculated as actions.Length / maxPages. If set to 0, maxPages must be more than 0.</param>
    /// <param name="maxPages">Maximum amount of pages that are allowed to be displayed. If this amount is reached, new perPage amount will be actions.Length / maxPages.</param>
    /// <returns>Array of actions in the selected by user page for you to draw.</returns>
    public static Action[] Pagination(string id, Action[] actions, out Action? paginator, int perPage = 0, int maxPages = 0)
    {
        if(actions.Length == 0)
        {
            paginator = null;
            return [];
        }
        if(perPage <= 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxPages);
            perPage = (int)MathF.Ceiling((float)actions.Length / (float)maxPages);
        }
        else
        {
            var newMaxPages = (int)MathF.Ceiling((float)actions.Length / (float)perPage);
            if(maxPages > 0 && newMaxPages > maxPages)
            {
                perPage = (int)MathF.Ceiling((float)actions.Length / (float)maxPages);
            }
            else
            {
                maxPages = newMaxPages;
            }
        }
        if(perPage >= actions.Length)
        {
            paginator = null;
            return actions;
        }
        if(!SelectedPages.ContainsKey(id)) SelectedPages[id] = 0;
        if(!SelectedPages[id].InRange(0, maxPages, false)) SelectedPages[id] = 0;
        void Paginator()
        {
            ImGui.PushID(id);
            var width = ImGui.GetContentRegionAvail().X / maxPages;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 1));
            for(var i = 0; i < maxPages; i++)
            {
                if(i == maxPages - 1) width = ImGui.GetContentRegionAvail().X;
                var act = SelectedPages[id] == i;
                if(act)
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonActive]);
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonActive]);
                }
                if(ImGui.Button($"{i + 1}", new(width, ImGui.GetFrameHeight())))
                {
                    SelectedPages[id] = i;
                }
                if(act) ImGui.PopStyleColor(2);
                if(i != maxPages - 1) ImGui.SameLine(0, 0);
            }
            ImGui.PopStyleVar(2);
            ImGui.PopID();
        }
        var rangeMin = SelectedPages[id] * perPage;
        var rangeMax = (SelectedPages[id] + 1) * perPage - 1;
        if(rangeMax > actions.Length) rangeMax = actions.Length;
        paginator = Paginator;
        return actions[rangeMin..rangeMax];
    }

    public static void TreeNodeCollapsingHeader(string name, Action action, ImGuiTreeNodeFlags extraFlags = ImGuiTreeNodeFlags.None) => TreeNodeCollapsingHeader(name, true, action, extraFlags);
    /// <summary>
    /// Another interpretation of <see cref="ImGui.CollapsingHeader(string)"/> but with narrow design and border.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="usePadding"></param>
    /// <param name="action"></param>
    /// <param name="extraFlags"></param>
    public static void TreeNodeCollapsingHeader(string name, bool usePadding, Action action, ImGuiTreeNodeFlags extraFlags = ImGuiTreeNodeFlags.None)
    {
        ImGui.PushID("CollapsingHeaderHelperTable");
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, Vector2.Zero);
        if(ImGui.BeginTable($"{name}", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoSavedSettings))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            var ret = ImGuiEx.TreeNode(name, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Selected | extraFlags);
            ImGui.PopStyleVar();
            if(ret)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if(ImGui.BeginTable($"2{name}", 1, ImGuiTableFlags.NoSavedSettings | (usePadding?ImGuiTableFlags.PadOuterX:ImGuiTableFlags.None)))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    try
                    {
                        action();
                    }
                    catch(Exception e)
                    {
                        e.Log();
                    }
                    ImGui.EndTable();
                }
            }
            ImGui.EndTable();
        }
        else
        {
            ImGui.PopStyleVar();
        }
        ImGui.PopID();
    }

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

    /// <summary>
    /// Draws plugin availability checkmark. Allows to check by name and version. When hovered, will display tooltip with info about which plugins are installed, outdated or missing.
    /// </summary>
    /// <param name="pluginInfos">RequiredPluginInfos of plugins that are required</param>
    /// <param name="prependText">Override first tooltip line if you want</param>
    /// <param name="all">Whether to check for all plugins from the list or just one of them</param>
    public static void PluginAvailabilityIndicator(IEnumerable<RequiredPluginInfo> pluginInfos, string? prependText = null, bool all = true)
    {
        prependText ??= all?"The following plugins are required to be installed and enabled:":"One of the following plugins is required to be installed and enabled";
        bool pass;
        if(all)
        {
            pass = pluginInfos.All(info => Svc.PluginInterface.InstalledPlugins.Any(x => x.IsLoaded && x.InternalName == info.InternalName && (info.MinVersion == null || x.Version >= info.MinVersion)));
        }
        else
        {
            pass = pluginInfos.Any(info => Svc.PluginInterface.InstalledPlugins.Any(x => x.IsLoaded && x.InternalName == info.InternalName && (info.MinVersion == null || x.Version >= info.MinVersion)));
        }

        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(pass ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, pass ? FontAwesomeIcon.Check.ToIconString() : "\uf00d");
        ImGui.PopFont();
        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGuiEx.Text(prependText);
            ImGui.PopTextWrapPos();
            foreach(var info in pluginInfos)
            {
                var plugin = Svc.PluginInterface.InstalledPlugins.FirstOrDefault(x => x.IsLoaded && x.InternalName == info.InternalName);
                if(plugin != null)
                {
                    if(info.MinVersion == null || plugin.Version >= info.MinVersion)
                    {
                        ImGuiEx.Text(ImGuiColors.ParsedGreen, $"- {info.VanityName ?? info.InternalName}" + (info.MinVersion == null ? "" : $" {info.MinVersion}+"));
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
                    ImGuiEx.Text(ImGuiColors.DalamudRed, $"- {info.VanityName ?? info.InternalName} " + (info.MinVersion == null ? "" : $"{info.MinVersion}+ "));
                    ImGui.SameLine(0, 0);
                    ImGuiEx.Text($"(not installed)");
                }
            }
            ImGui.EndTooltip();
        }

    }

    /// <summary>Selectable item made from TreeNode with bullet mark in front</summary>
    /// <inheritdoc cref="Selectable(Vector4?, string, ref bool, ImGuiTreeNodeFlags)"/>
    public static bool Selectable(Vector4? color, string id)
    {
        var ret = ImGuiEx.TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Leaf);
        return ret;
    }

    /// <inheritdoc cref="Selectable(Vector4?, string)"/>
    public static bool Selectable(string id) => Selectable(null, id);

    /// <inheritdoc cref="Selectable(Vector4?, string)"/>
    public static bool Selectable(string id, ref bool selected) => Selectable(null, id, ref selected);


    /// <summary>
    /// Selectable item made from TreeNode
    /// </summary>
    /// <param name="color">Text color</param>
    /// <param name="id">ImGui ID</param>
    /// <param name="selected">Selected state storage field</param>
    /// <param name="extraFlags">Extra tree node flags</param>
    /// <returns><see langword="true"/> when clicked</returns>
    public static bool Selectable(Vector4? color, string id, ref bool selected, ImGuiTreeNodeFlags extraFlags = ImGuiTreeNodeFlags.Leaf)
    {
        ImGuiEx.TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | (selected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) | extraFlags);
        var ret = ImGui.IsItemClicked(ImGuiMouseButton.Left);
        if(ret) selected = !selected;
        return ret;
    }

    ///<inheritdoc cref="TreeNode(Vector4?, string, ImGuiTreeNodeFlags)"/>
    public static bool TreeNode(string name, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanFullWidth) => ImGuiEx.TreeNode(null, name, flags);

    /// <summary>
    /// Just like <see cref="ImGui.TreeNode"/> but with color option and that spans full width by default.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="name"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool TreeNode(Vector4? color, string name, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanFullWidth)
    {
        flags |= ImGuiTreeNodeFlags.SpanFullWidth;
        if(color != null) ImGui.PushStyleColor(ImGuiCol.Text, color.Value);
        var ret = ImGui.TreeNodeEx(name, flags);
        if(color != null) ImGui.PopStyleColor();
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

    private static string JobSelectorFilter = "";
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
        if(ImGui.BeginCombo(id, preview))
        {
            if(ImGui.IsWindowAppearing() && options?.Contains(JobSelectorOption.ClearFilterOnOpen) == true)
                ImGui.SetNextItemWidth(150f);
            ImGui.InputTextWithHint("##filter", "Filter...", ref JobSelectorFilter, 50);
            foreach(var cond in Enum.GetValues<Job>().Where(x => baseJobs || !x.IsUpgradeable()).OrderByDescending(x => Svc.Data.GetExcelSheet<ClassJob>().GetRow((uint)x).Role))
            {
                if(cond == Job.ADV) continue;
                if(jobDisplayFilter != null && !jobDisplayFilter(cond)) continue;
                var name = cond.ToString().Replace("_", " ");
                if(JobSelectorFilter == "" || name.Contains(JobSelectorFilter, StringComparison.OrdinalIgnoreCase))
                {
                    if(ThreadLoadImageHandler.TryGetIconTextureWrap((uint)cond.GetIcon(), false, out var texture))
                    {
                        ImGui.Image(texture.ImGuiHandle, new Vector2(24f));
                        ImGui.SameLine();
                    }
                    if(ImGuiEx.CollectionCheckbox(name, cond, selectedJobs)) ret = true;
                }
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    ///<inheritdoc cref="InfoMarker(string, Vector4?, string, bool)"/>
    public static void HelpMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true) => InfoMarker(helpText, color, symbolOverride, sameLine);

    /// <summary>
    /// <see cref="ImGuiComponents.HelpMarker(string)"/> but with more options
    /// </summary>
    /// <param name="helpText"></param>
    /// <param name="color"></param>
    /// <param name="symbolOverride"></param>
    /// <param name="sameLine">Whether to call SameLine before drawing marker</param>
    public static void InfoMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true)
    {
        if(sameLine) ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(color ?? ImGuiColors.DalamudGrey3, symbolOverride ?? FontAwesomeIcon.InfoCircle.ToIconString());
        ImGui.PopFont();
        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGui.TextUnformatted(helpText);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

    /// <summary>
    /// <see cref="ImGui.SliderInt"/> but with double-click to edit support.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="v"></param>
    /// <param name="v_min"></param>
    /// <param name="v_max"></param>
    /// <param name="format"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool SliderInt(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max, format, flags);
        ActivateIfDoubleClicked();
        return ret;
    }

    ///<inheritdoc cref="SliderInt(string, ref int, int, int, string, ImGuiSliderFlags)"/>
    public static bool SliderInt(string label, ref int v, int v_min, int v_max, string format)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max, format);
        ActivateIfDoubleClicked();
        return ret;
    }

    ///<inheritdoc cref="SliderInt(string, ref int, int, int, string, ImGuiSliderFlags)"/>
    public static bool SliderInt(string label, ref int v, int v_min, int v_max)
    {
        var ret = ImGui.SliderInt(label, ref v, v_min, v_max);
        ActivateIfDoubleClicked();
        return ret;
    }

    /// <summary>
    /// <see cref="ImGui.SliderFloat"/> but with double-click to edit support.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="v"></param>
    /// <param name="v_min"></param>
    /// <param name="v_max"></param>
    /// <param name="format"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max, format, flags);
        ActivateIfDoubleClicked();
        return ret;
    }

    ///<inheritdoc cref="SliderFloat(string, ref float, float, float, string, ImGuiSliderFlags)"/>
    public static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max, format);
        ActivateIfDoubleClicked();
        return ret;
    }

    ///<inheritdoc cref="SliderFloat(string, ref float, float, float, string, ImGuiSliderFlags)"/>
    public static bool SliderFloat(string label, ref float v, float v_min, float v_max)
    {
        var ret = ImGui.SliderFloat(label, ref v, v_min, v_max);
        ActivateIfDoubleClicked();
        return ret;
    }

    /// <summary>
    /// Activates item when double-clicked. Place after any ImGui Slider component to enable edit on double-click.
    /// </summary>
    public static void ActivateIfDoubleClicked()
    {
        if(ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            ImGui.SetKeyboardFocusHere(-1);
        }
    }

    public static void SetNextItemWidthScaled(float width)
    {
        ImGui.SetNextItemWidth(width.Scale());
    }

    private static int FindWrapPosition(string text, float wrapWidth)
    {
        float currentWidth = 0;
        int lastSpacePos = -1;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            currentWidth += ImGui.CalcTextSize(c.ToString()).X;
            if (char.IsWhiteSpace(c))
            {
                lastSpacePos = i;
            }
            if (currentWidth > wrapWidth)
            {
                return lastSpacePos >= 0 ? lastSpacePos : i;
            }
        }
        return -1;
    }

    private static unsafe int TextEditCallback(ImGuiInputTextCallbackData* data, float wrapWidth)
    {
        string text = Marshal.PtrToStringAnsi((IntPtr)data->Buf, data->BufTextLen);
        var lines = text.Split('\n').ToList();
        bool textModified = false;
        // Traverse each line to check if it exceeds the wrap width
        for (int i = 0; i < lines.Count; i++)
        {
            float lineWidth = ImGui.CalcTextSize(lines[i]).X;
            while (lineWidth + 10f > wrapWidth)
            {
                // Find where to break the line
                int wrapPos = FindWrapPosition(lines[i], wrapWidth);
                if (wrapPos >= 0)
                {
                    // Insert a newline at the wrap position
                    string part1 = lines[i].Substring(0, wrapPos);
                    string part2 = lines[i].Substring(wrapPos).TrimStart();
                    lines[i] = part1;
                    lines.Insert(i + 1, part2);
                    textModified = true;
                    lineWidth = ImGui.CalcTextSize(part2).X;
                }
                else
                {
                    break;
                }
            }
        }
        // Merge all lines back to the buffer
        if (textModified)
        {
            string newText = string.Join("\n", lines);
            byte[] newTextBytes = Encoding.UTF8.GetBytes(newText.PadRight(data->BufSize, '\0'));
            Marshal.Copy(newTextBytes, 0, (IntPtr)data->Buf, newTextBytes.Length);
            data->BufTextLen = newText.Length;
            data->BufDirty = 1;
            data->CursorPos = Math.Min(data->CursorPos, data->BufTextLen);
        }
        return 0;
    }

    public unsafe static bool InputTextWrapMultilineExpanding(string id, ref string text, uint maxLength = 500, int minLines = 2, int maxLines = 10, int? width = null)
    {
        float wrapWidth = width ?? ImGui.GetContentRegionAvail().X; // determine wrap width
        bool result = ImGui.InputTextMultiline(id, ref text, maxLength,
            new(width ?? ImGui.GetContentRegionAvail().X, ImGui.CalcTextSize("A").Y * Math.Clamp(text.Split("\n").Length + 1, minLines, maxLines)),
            ImGuiInputTextFlags.CallbackEdit, // flag stuff 
            (data) => {
                return TextEditCallback(data, wrapWidth); // Callback Action
            });
        return result;
    }

    public static bool InputTextMultilineExpanding(string id, ref string text, uint maxLength = 500, int minLines = 2, int maxLines = 10, int? width = null)
    {
        return ImGui.InputTextMultiline(id, ref text, maxLength, new(width ?? ImGui.GetContentRegionAvail().X, ImGui.CalcTextSize("A").Y * Math.Clamp(text.Split("\n").Length + 1, minLines, maxLines)));
    }

    public static bool EnumOrderer<T>(string id, List<T> order) where T : IConvertible
    {
        var ret = false;
        var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        foreach(var x in enumValues) if(!order.Contains(x)) order.Add(x);
        if(order.Count > enumValues.Length)
        {
            PluginLog.Warning($"EnumOrderer: duplicates or non-existing items found, enum {enumValues.Print()}, list {order.Print()}, cleaning up.");
            order.RemoveAll(x => !enumValues.Contains(x));
            var set = order.ToHashSet();
            order.Clear();
            order.AddRange(set);
        }
        for(var i = 0; i < order.Count; i++)
        {
            var e = order[i];
            ImGui.PushID($"ECommonsEnumOrderer{id}{e}");
            if(ImGui.ArrowButton("up", ImGuiDir.Up) && i > 0)
            {
                (order[i - 1], order[i]) = (order[i], order[i - 1]);
                ret = true;
            }
            ImGui.SameLine();
            if(ImGui.ArrowButton("down", ImGuiDir.Down) && i < order.Count - 1)
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

    /// <summary>
    /// Checks whether item is hovered and double-clicked. Sets cursor to hand to indicate that it can be clicked.
    /// </summary>
    /// <param name="tooltip">Optional tooltip</param>
    /// <param name="btn">Which button to check</param>
    /// <param name="requireCtrl">Whether to require CTRL when clicking</param>
    /// <returns></returns>
    public static bool HoveredAndClicked(string tooltip = null, ImGuiMouseButton btn = ImGuiMouseButton.Left, bool requireCtrl = false)
    {
        if(ImGui.IsItemHovered() && ImGui.GetMouseDragDelta().X < 2f && ImGui.GetMouseDragDelta().Y < 2f)
        {
            if(tooltip != null)
            {
                SetTooltip(tooltip);
            }
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            return (!requireCtrl || ImGui.GetIO().KeyCtrl) && ImGui.IsMouseReleased(btn);
        }
        return false;
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

    public static bool InputLong(string id, ref long num)
    {
        var txt = num.ToString();
        var ret = ImGui.InputText(id, ref txt, 50);
        long.TryParse(txt, out num);
        return ret;
    }

    public static bool CollapsingHeader(string text, Vector4? col = null)
    {
        if(col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        var ret = ImGui.CollapsingHeader(text);
        if(col != null) ImGui.PopStyleColor();
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

    /// <summary>
    /// Converts RGB color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBB</param>
    /// <param name="alpha">Optional transparency value between 0 and 1</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGB(this uint col, float alpha = 1.0f)
    {
        var bytes = (byte*)&col;
        return new Vector4((float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f, alpha);
    }

    /// <summary>
    /// Converts RGBA color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBBAA</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGBA(this uint col)
    {
        var bytes = (byte*)&col;
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
        var col = !inverted?value:!value;
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

    public static bool CollectionButtonCheckbox<T>(string name, T value, ICollection<T> collection, bool smallButton = false, bool inverted = false) => CollectionButtonCheckbox(name, value, collection, EColor.Red, smallButton, inverted);

    public static bool CollectionButtonCheckbox<T>(string name, T value, ICollection<T> collection, Vector4 color, bool smallButton = false, bool inverted = false)
    {
        var col = collection.Contains(value);
        if(inverted) col = !col;
        var ret = false;
        if(col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if(smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if(col)
            {
                if(inverted)
                {
                    collection.Add(value);
                }
                else
                {
                    collection.Remove(value);
                }
            }
            else
            {
                if(inverted)
                {
                    collection.Remove(value);
                }
                else
                {
                    collection.Add(value);
                }
            }
            ret = true;
        }
        if(col) ImGui.PopStyleColor(3);
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
        if(ImGui.RadioButton(labelTrue, value)) value = true;
        suffix?.Invoke();
        if(sameLine) ImGui.SameLine();
        prefix?.Invoke();
        if(ImGui.RadioButton(labelFalse, !value)) value = false;
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
        if(values.Length == 1)
        {
            GenericHelpers.Safe(values[0]);
        }
        else
        {
            if(ImGui.BeginTable(id, Math.Max(1, columns ?? values.Length), ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.NoSavedSettings | extraFlags))
            {
                foreach(var action in values)
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
        if(ImGui.IsWindowCollapsed()) return false;

        var scale = ImGuiHelpers.GlobalScale;
        var currentID = ImGui.GetID(0);
        if(currentID != headerLastWindowID || headerLastFrame != Svc.PluginInterface.UiBuilder.FrameCount)
        {
            headerLastWindowID = currentID;
            headerLastFrame = Svc.PluginInterface.UiBuilder.FrameCount;
            headerCurrentPos = 0.25f * ImGui.GetStyle().FramePadding.Length();
            if(!GetCurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoTitleBar))
                headerCurrentPos = 1;
            headerImGuiButtonWidth = 0f;
            if(CurrentWindowHasCloseButton())
                headerImGuiButtonWidth += 17 * scale;
            if(!GetCurrentWindowFlags().HasFlag(ImGuiWindowFlags.NoCollapse))
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
        if(ImGui.IsWindowHovered() && ImGui.IsMouseHoveringRect(itemMin, itemMax, false))
        {
            if(!string.IsNullOrEmpty(options.Tooltip))
                ImGui.SetTooltip(options.Tooltip);
            ImGui.GetWindowDrawList().AddCircleFilled(center, halfSize.X, ImGui.GetColorU32(ImGui.IsMouseDown(ImGuiMouseButton.Left) ? ImGuiCol.ButtonActive : ImGuiCol.ButtonHovered));
            if(ImGui.IsMouseReleased(options.MouseButton))
                pressed = true;
#pragma warning disable
            if(options.ToastTooltipOnClick && ImGui.IsMouseReleased(options.ToastTooltipOnClickButton))
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
        if(ret)
        {
            value = (int)(f * divider);
        }
        return ret;
    }

    public static bool IsKeyPressed(int key, bool repeat)
    {
        var repeat2 = (byte)(repeat ? 1 : 0);
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

    private static Dictionary<string, Box<string>> InputListValuesString = [];
    public static void InputListString(string name, List<string> list, Dictionary<string, string> overrideValues = null, string defaultValue = null)
    {
        if(!InputListValuesString.ContainsKey(name)) InputListValuesString[name] = new("");
        InputList(name, list, overrideValues, delegate
        {
            var buttonSize = ImGuiHelpers.GetButtonSize("Add");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
            ImGui.InputText($"##{name.Replace("#", "_")}", ref InputListValuesString[name].Value, 100);
            ImGui.SameLine();
            if(ImGui.Button("Add"))
            {
                list.Add(InputListValuesString[name].Value);
                InputListValuesString[name].Value = "";
            }
        });
    }

    private static Dictionary<string, Box<uint>> InputListValuesUint = [];
    public static void InputListUint(string name, List<uint> list, Dictionary<uint, string> overrideValues = null)
    {
        if(!InputListValuesUint.ContainsKey(name)) InputListValuesUint[name] = new(0);
        InputList(name, list, overrideValues, delegate
        {
            var buttonSize = ImGuiHelpers.GetButtonSize("Add");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
            ImGuiEx.InputUint($"##{name.Replace("#", "_")}", ref InputListValuesUint[name].Value);
            ImGui.SameLine();
            if(ImGui.Button("Add"))
            {
                list.Add(InputListValuesUint[name].Value);
                InputListValuesUint[name].Value = 0;
            }
        });
    }

    public static void InputList<T>(string name, List<T> list, Dictionary<T, string> overrideValues, Action addFunction)
    {
        var text = list.Count == 0 ? "- No values -" : (list.Count == 1 ? $"{(overrideValues != null && overrideValues.ContainsKey(list[0]) ? overrideValues[list[0]] : list[0])}" : $"- {list.Count} elements -");
        if(ImGui.BeginCombo(name, text))
        {
            addFunction();
            var rem = -1;
            for(var i = 0; i < list.Count; i++)
            {
                var id = $"{name}ECommonsDeleItem{i}";
                var x = list[i];
                ImGui.Selectable($"{(overrideValues != null && overrideValues.ContainsKey(x) ? overrideValues[x] : x)}");
                if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup(id);
                }
                if(ImGui.BeginPopup(id))
                {
                    if(ImGui.Selectable("Delete##ECommonsDeleItem"))
                    {
                        rem = i;
                    }
                    if(ImGui.Selectable("Clear (hold shift+ctrl)##ECommonsDeleItem")
                        && ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl)
                    {
                        rem = -2;
                    }
                    ImGui.EndPopup();
                }
            }
            if(rem > -1)
            {
                list.RemoveAt(rem);
            }
            if(rem == -2)
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
        if(ImGui.IsItemHovered())
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
        ImGui.AlignTextToFramePadding();
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
        if(percent < 25)
        {
            return ImGuiColors.ParsedGrey;
        }
        else if(percent < 50)
        {
            return ImGuiColors.ParsedGreen;
        }
        else if(percent < 75)
        {
            return ImGuiColors.ParsedBlue;
        }
        else if(percent < 95)
        {
            return ImGuiColors.ParsedPurple;
        }
        else if(percent < 99)
        {
            return ImGuiColors.ParsedOrange;
        }
        else if(percent == 99)
        {
            return ImGuiColors.ParsedPink;
        }
        else if(percent == 100)
        {
            return ImGuiColors.ParsedGold;
        }
        else
        {
            return ImGuiColors.DalamudRed;
        }
    }

    /// <summary>
    /// Draws a checkbox that will be on the same line as previous if there is space, otherwise will move to the next line.
    /// </summary>
    /// <param name="label">Checkbox label</param>
    /// <param name="v">Boolean to toggle</param>
    /// <remarks><see cref="ImGui.SameLine()"/> does not need to be called just before using this.</remarks>
    /// <returns></returns>
    public static bool CheckboxWrapped(string label, ref bool v)
    {
        ImGui.SameLine();
        var labelW = ImGui.CalcTextSize(label);
        var finishPos = ImGui.GetCursorPosX() + labelW.X + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().ItemInnerSpacing.X + ImGui.GetStyle().FramePadding.Length() + ImGui.GetCursorStartPos().X;
        if (finishPos >= ImGui.GetContentRegionMax().X)
            ImGui.NewLine();

        return ImGui.Checkbox(label, ref v);
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
        if (finishPos >= ImGui.GetContentRegionMax().X)
            ImGui.NewLine();

        return ImGui.Button(label);
    }

    public static void EzTabBar(string id, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, openTabName, ImGuiTabBarFlags.None, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, ImGuiTabBarFlags flags, params (string name, Action function, Vector4? color, bool child)[] tabs)
    {
        ImGui.BeginTabBar(id, flags);
        foreach(var x in tabs)
        {
            if(x.name == null) continue;
            if(x.color != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, x.color.Value);
            }
            if(ImGuiEx.BeginTabItem(x.name, openTabName == x.name ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None))
            {
                if(x.color != null)
                {
                    ImGui.PopStyleColor();
                }
                if(x.child) ImGui.BeginChild(x.name + "child");
                x.function();
                if(x.child) ImGui.EndChild();
                ImGui.EndTabItem();
            }
            else
            {
                if(x.color != null)
                {
                    ImGui.PopStyleColor();
                }
            }
        }
        if(KoFiTransparent != null) PatreonBanner.RightTransparentTab();
        ImGui.EndTabBar();
    }

    public static void InvisibleButton(int width = 0)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button(" ");
        ImGui.PopStyleVar();
    }

    public static Dictionary<string, Box<string>> EnumComboSearch = [];

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
        if(ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString().Replace("_", " ")))
        {
            var values = Enum.GetValues(typeof(T));
            Box<string> fltr = null;
            if(values.Length > 10)
            {
                if(!EnumComboSearch.ContainsKey(name)) EnumComboSearch.Add(name, new(""));
                fltr = EnumComboSearch[name];
                ImGuiEx.SetNextItemFullWidth();
                ImGui.InputTextWithHint($"##{name.Replace("#", "_")}", "Filter...", ref fltr.Value, 50);
            }
            foreach(var x in values)
            {
                var equals = EqualityComparer<T>.Default.Equals((T)x, refConfigField);
                var element = (names != null && names.TryGetValue((T)x, out n)) ? n : x.ToString().Replace("_", " ");
                if((filter == null || filter((T)x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Selectable(element, equals)
                    )
                {
                    ret = true;
                    refConfigField = (T)x;
                }
                if(ImGui.IsWindowAppearing() && equals) ImGui.SetScrollHereY();
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    public static bool EnumRadio<T>(ref T refConfigField, bool sameLine = false, Func<T, bool> filter = null, IDictionary<T, string> names = null) where T : Enum, IConvertible
    {
        var ret = false;
        var values = Enum.GetValues(typeof(T));
        var first = true;
        foreach(var x in values)
        {
            if(!first && sameLine) ImGui.SameLine();
            first = false;
            var equals = EqualityComparer<T>.Default.Equals((T)x, refConfigField);
            var element = (names != null && names.TryGetValue((T)x, out var n)) ? n : x.ToString().Replace("_", " ");
            if((filter == null || filter((T)x))
                && ImGui.RadioButton(element, equals)
                )
            {
                ret = true;
                refConfigField = (T)x;
            }
        }
        return ret;
    }

    public static Dictionary<string, Box<string>> ComboSearch = [];
    public static bool Combo<T>(string name, ref T refConfigField, IEnumerable<T> values, Func<T, bool> filter = null, Dictionary<T, string> names = null)
    {
        var ret = false;
        if(ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString(), ImGuiComboFlags.HeightLarge))
        {
            Box<string> fltr = null;
            if(values.Count() > 10)
            {
                if(!ComboSearch.ContainsKey(name)) ComboSearch.Add(name, new(""));
                fltr = ComboSearch[name];
                ImGuiEx.SetNextItemFullWidth();
                ImGui.InputTextWithHint($"##{name}fltr", "Filter...", ref fltr.Value, 50);
            }
            foreach(var x in values)
            {
                var equals = EqualityComparer<T>.Default.Equals(x, refConfigField);
                var element = (names != null && names.TryGetValue(x, out n)) ? n : x.ToString();
                if((filter == null || filter(x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Selectable(element, equals)
                    )
                {
                    ret = true;
                    refConfigField = x;
                }
                if(ImGui.IsWindowAppearing() && equals) ImGui.SetScrollHereY();
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
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.SmallButton(label) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool Button(string label, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.Button(label) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool Button(string label, Vector2 size, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var ret = ImGui.Button(label, size) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    public static bool IconButton(string icon, string id = "ECommonsButton", Vector2 size = default, bool enabled = true)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var result = ImGui.Button($"{icon}##{icon}-{id}", size) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        ImGui.PopFont();
        return result;
    }

    public static bool IconButtonWithText(FontAwesomeIcon icon, string id, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);
        var result = ImGuiComponents.IconButtonWithText(icon, $"{id}") && enabled;
        if(!enabled) ImGui.PopStyleVar();
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
        if(ImGui.InputText(name, ref text, 50))
        {
            if(uint.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, null, out var num))
            {
                hexInt = num;
            }
        }
    }

    public static void InputHex(string name, ref long hexInt)
    {
        var text = $"{hexInt:X}";
        if(ImGui.InputText(name, ref text, 50))
        {
            if(long.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, null, out var num))
            {
                hexInt = num;
            }
        }
    }

    public static void InputHex(string name, ref byte hexByte)
    {
        var text = $"{hexByte:X}";
        if(ImGui.InputText(name, ref text, 2))
        {
            if(byte.TryParse(text, NumberStyles.HexNumber, null, out var num))
            {
                hexByte = num;
            }
        }
    }

    public static void InputUint(string name, ref uint uInt)
    {
        var text = $"{uInt}";
        if(ImGui.InputText(name, ref text, 16))
        {
            if(uint.TryParse(text, out var num))
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
        if(ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
#pragma warning disable
            GenericHelpers.Copy(text);
#pragma warning restore
        }
    }

    public static void TextWrappedCopy(string text)
    {
        ImGuiEx.TextWrapped(text);
        if(ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
#pragma warning disable
            GenericHelpers.Copy(text);
#pragma warning restore
        }
    }

    public static void TextWrappedCopy(Vector4 col, string text)
    {
        ImGuiEx.TextWrapped(col, text);
        if(ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
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
        if(col == null)
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
        if(ImGui.Button(buttonText.Replace("$COPY", copy)))
        {
#pragma warning disable
            GenericHelpers.Copy(copy);
#pragma warning restore
        }
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
        ImGui.PushStyleColor(ImGuiCol.Text, colour);
        CenterColumnText(text, underlined);
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Move the cursor by the vector relative to the current position
    /// </summary>
    public static void PushCursor(Vector2 vec) => ImGui.SetCursorPos(ImGui.GetCursorPos() + vec);
    /// <summary>
    /// Move the cursor by the coordinates relative to the current position
    /// </summary>
    public static void PushCursor(float x, float y) => PushCursor(new Vector2(x, y));
    /// <summary>
    /// Move the cursor horizontally x units relative to the current position
    /// </summary>
    public static void PushCursorX(float x) => ImGui.SetCursorPosX(ImGui.GetCursorPosX() + x);
    /// <summary>
    /// Move the cursor vertically y units relative to the current position
    /// </summary>
    public static void PushCursorY(float y) => ImGui.SetCursorPosY(ImGui.GetCursorPosY() + y);

    public static unsafe bool BeginTabItem(string label, ImGuiTabItemFlags flags)
    {
        var num = 0;
        byte* ptr;
        if(label != null)
        {
            num = Encoding.UTF8.GetByteCount(label);
            ptr = Allocate(num + 1);
            var utf = GetUtf8(label, ptr, num);
            ptr[utf] = 0;
        }
        else
        {
            ptr = null;
        }

        byte* p_open2 = null;
        var num2 = ImGuiNative.igBeginTabItem(ptr, p_open2, flags);
        if(num > 2048)
        {
            Free(ptr);
        }
        return num2 != 0;
    }

    internal static unsafe byte* Allocate(int byteCount)
    {
        return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
    }

    internal static unsafe void Free(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    internal static unsafe int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed(char* chars = s)
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

