using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.Funding;
using ECommons.Logging;
using ECommons.MathHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using TerraFX.Interop.Windows;
using Action = System.Action;

namespace ECommons.ImGuiMethods;
#nullable disable

public static unsafe partial class ImGuiEx
{
    public static readonly ImGuiWindowFlags OverlayFlags = ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing;
    /// <summary>
    /// Flags that are used for <see cref="BeginDefaultTable"/>. You can change them, if you want.
    /// </summary>
    public static ImGuiTableFlags DefaultTableFlags = ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit;
    private static Dictionary<string, int> SelectedPages = [];

    /// <summary>
    /// Fully equals to <see cref="ImGui.DragFloat"/>.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="v"></param>
    /// <param name="vSpeed"></param>
    /// <param name="vMin"></param>
    /// <param name="vMax"></param>
    /// <param name="format"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool DragDouble(ImU8String label, scoped ref double v, float vSpeed = 1.0f, float vMin = 0.0f, float vMax = 0.0f, ImU8String format = default, ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        var f = (float)v;
        var ret = ImGui.DragFloat(label, ref f, vSpeed, vMin, vMax, format, flags);
        if(ret)
        {
            v = f;
        }
        return ret;
    }

    /// <summary>
    /// ArrowButton that was removed in new bindings.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static bool ArrowButton(string label, ImGuiDir direction)
    {
        if(label.IsNullOrEmpty()) label = "ECommonsDefaultID";
        byte[] utf8Bytes = [..Encoding.UTF8.GetBytes(label), 0];

        fixed(byte* pUtf8 = utf8Bytes)
        {
            return ImGuiNative.ArrowButton(pUtf8, direction) != 0;
        }
    }

    [Obsolete("Switch to ImGui.PushId", true)]
    public static void PushID(string id)
    {
        if(id == null || id.Length == 0)
        {
            ImGui.PushID($"ECommonsDefaultID");
        }
        else
        {
            ImGui.PushID(id);
        }
    }

    /// <summary>
    /// <see cref="ImGui.InputTextWithHint"/> but you do not need to have a field. 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="hint"></param>
    /// <param name="result"></param>
    /// <param name="maxLength"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool FilteringInputTextWithHint(string label, string hint, out string result, int maxLength = 200, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        var ret = false;
        ref var value = ref GetFilteringInputTextString(label);
        if(ImGui.InputTextWithHint(label, hint, ref value, maxLength, flags))
        {
            ret = true;
        }
        result = value;
        return ret;
    }

    public static ref string GetFilteringInputTextString(string label) => ref Ref<string>.Get($"{ImGui.GetID(label)}_filter");

    /// <summary>
    /// <see cref="ImGui.InputInt"/> but you do not need to have a field. 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="result"></param>
    /// <param name="step"></param>
    /// <param name="step_fast"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool FilteringInputInt(string label, out int result, int step = 1, int step_fast = 100, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        var ret = false;
        ref var value = ref Ref<int>.Get($"{ImGui.GetID(label)}_filter");
        if(ImGui.InputInt(label, ref value, step, step_fast, flags:flags))
        {
            ret = true;
        }
        result = value;
        return ret;
    }

    /// <summary>
    /// <see cref="ImGui.Checkbox"/> but you do not need to have a field. 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool FilteringCheckbox(string label, out bool result)
    {
        var ret = false;
        ref var value = ref Ref<bool>.Get($"{ImGui.GetID(label)}_filter");
        if(ImGui.Checkbox(label, ref value))
        {
            ret = true;
        }
        result = value;
        return ret;
    }

    /// <summary>
    /// Allows you to create simple "drag this item to quickly copy it" to allow user to quickly mass change it. Use it in for/foreach loops when drawing ImGui elements.
    /// </summary>
    /// <typeparam name="T">Must be a struct. For classes, there is <see cref="ImGuiEx.DragDropRepopulateClass{T}(string, T, Action{T})"/>.</typeparam>
    /// <param name="dragDropIdentifier">Plugin-unique identifier, internal and invisible to used. Must be relatively short. </param>
    /// <param name="data">Element's current data</param>
    /// <param name="callback">A callback that will be executed when one item is dragged onto another. It's parameter is a value of an item that is dragged. Assign it to the current item. </param>
    public static void DragDropRepopulate<T>(string dragDropIdentifier, T data, Action<T> callback) where T : struct
    {
        ImGuiEx.Tooltip("Drag this selector to other selectors to set their values to the same");
        if(ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoPreviewTooltip))
        {
            try
            {
                ImGuiDragDrop.SetDragDropPayload<T>(dragDropIdentifier, data);
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
            }
            catch(Exception e)
            {
                e.Log();
            }
            ImGui.EndDragDropSource();
        }
        if(ImGui.BeginDragDropTarget())
        {
            try
            {
                if(ImGuiDragDrop.AcceptDragDropPayload<T>(dragDropIdentifier, out var outId, ImGuiDragDropFlags.AcceptBeforeDelivery | ImGuiDragDropFlags.AcceptNoPreviewTooltip))
                {
                    callback(outId);
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
            ImGui.EndDragDropTarget();
        }
    }

    /// <summary>
    /// Allows you to create simple "drag this item to quickly copy it" to allow user to quickly mass change it. Use it in for/foreach loops when drawing ImGui elements.
    /// </summary>
    /// <typeparam name="T">Must be a class. For structs, there is <see cref="ImGuiEx.DragDropRepopulate{T}(string, T, Action{T})"/> and <see cref="ImGuiEx.DragDropRepopulate{T}(string, T, ref T)"/>.</typeparam>
    /// <param name="dragDropIdentifier">Plugin-unique identifier, internal and invisible to used. Must be relatively short. </param>
    /// <param name="data">Element's current data</param>
    /// <param name="callback">A callback that will be executed when one item is dragged onto another. It's parameter is a value of an item that is dragged. Assign it to the current item. </param>
    public static void DragDropRepopulateClass<T>(string dragDropIdentifier, T data, Action<T> callback) where T : class
    {
        var table = Ref<ConditionalWeakTable<T, Box<Guid>>>.Get("__ECommons.DragDropRepopulateClass.ConditionalWeakTable", () => new ConditionalWeakTable<T, Box<Guid>>());
        table.TryAdd(data, new(Guid.NewGuid()));
        if(table.TryGetValue(data, out var box))
        {
            ImGuiEx.Tooltip("Drag this selector to other selectors to set their values to the same");
            if(ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoPreviewTooltip))
            {
                try
                {
                    ImGuiDragDrop.SetDragDropPayload<Guid>(dragDropIdentifier, box.Value);
                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
                }
                catch(Exception e)
                {
                    e.Log();
                }
                ImGui.EndDragDropSource();
            }
            if(ImGui.BeginDragDropTarget())
            {
                try
                {
                    if(ImGuiDragDrop.AcceptDragDropPayload<Guid>(dragDropIdentifier, out var outId, ImGuiDragDropFlags.AcceptBeforeDelivery | ImGuiDragDropFlags.AcceptNoPreviewTooltip))
                    {
                        foreach(var x in table)
                        {
                            if(x.Value.Value == outId)
                            {
                                callback(x.Key);
                                break;
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    e.Log();
                }
                ImGui.EndDragDropTarget();
            }
        }
    }

    /// <summary>
    /// Allows you to create simple "drag this item to quickly copy it" to allow user to quickly mass change it. Use it in for/foreach loops when drawing ImGui elements. This overload is to be used with <see cref="CollectionCheckbox"/> and similar elements that are supposed to perform either addition or removal of element to/from collection.
    /// </summary>
    /// <typeparam name="T">Must be a struct. For classes, there is <see cref="ImGuiEx.DragDropRepopulateClass{T}(string, T, ICollection{T})"/>.</typeparam>
    /// <param name="dragDropIdentifier">Plugin-unique identifier, internal and invisible to used. Must be relatively short. </param>
    /// <param name="data">Element's current data</param>
    /// <param name="dataCollection">A collection where data will be added or removed from</param>
    public static void DragDropRepopulate<T>(string dragDropIdentifier, T data, ICollection<T> dataCollection) where T : struct
    {
        ImGuiEx.DragDropRepopulate(dragDropIdentifier, data, c =>
        {
            var cond = !dataCollection.Contains(c);
            if(cond)
            {
                dataCollection.Remove(data);
            }
            else
            {
                dataCollection.Add(data);
            }
        });
    }

    /// <summary>
    /// Allows you to create simple "drag this item to quickly copy it" to allow user to quickly mass change it. Use it in for/foreach loops when drawing ImGui elements. This overload is to be used with <see cref="CollectionCheckbox"/> and similar elements that are supposed to perform either addition or removal of element to/from collection.
    /// </summary>
    /// <typeparam name="T">Must be a class. For structs, there is <see cref="ImGuiEx.DragDropRepopulate{T}(string, T, ICollection{T})"/>.</typeparam>
    /// <param name="dragDropIdentifier">Plugin-unique identifier, internal and invisible to used. Must be relatively short. </param>
    /// <param name="data">Element's current data</param>
    /// <param name="dataCollection">A collection where data will be added or removed from</param>
    public static void DragDropRepopulateClass<T>(string dragDropIdentifier, T data, ICollection<T> dataCollection) where T : class
    {
        ImGuiEx.DragDropRepopulateClass(dragDropIdentifier, data, c =>
        {
            if(!dataCollection.Contains(c))
            {
                dataCollection.Remove(data);
            }
            else
            {
                dataCollection.Add(data);
            }
        });
    }

    /// <summary>
    /// Allows you to create simple "drag this item to quickly copy it" to allow user to quickly mass change it. Use it in for/foreach loops when drawing ImGui elements.
    /// </summary>
    /// <typeparam name="T">Must be a struct. For classes, there is <see cref="ImGuiEx.DragDropRepopulateClass{T}(string, T, Action{T})"/>.</typeparam>
    /// <param name="dragDropIdentifier">Plugin-unique identifier, internal and invisible to used. Must be relatively short. </param>
    /// <param name="data">Element's current data</param>
    /// <param name="field">A field which will be assigned data value when dragged onto.</param>
    public static void DragDropRepopulate<T>(string dragDropIdentifier, T data, ref T field) where T : struct
    {
        ImGuiEx.Tooltip("Drag this selector to other selectors to set their values to the same");
        if(ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoPreviewTooltip))
        {
            try
            {
                ImGuiDragDrop.SetDragDropPayload<T>(dragDropIdentifier, data);
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
            }
            catch(Exception e)
            {
                e.Log();
            }
            ImGui.EndDragDropSource();
        }
        if(ImGui.BeginDragDropTarget())
        {
            try
            {
                if(ImGuiDragDrop.AcceptDragDropPayload<T>(dragDropIdentifier, out var outId, ImGuiDragDropFlags.AcceptBeforeDelivery | ImGuiDragDropFlags.AcceptNoPreviewTooltip))
                {
                    field = outId;
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
            ImGui.EndDragDropTarget();
        }
    }

    /// <inheritdoc cref="Scale(float)"/>
    public static Vector2? Scale(this Vector2? v)
    {
        if(v == null) return null;
        return new Vector2(v.Value.X.Scale(), v.Value.Y.Scale());
    }

    /// <inheritdoc cref="Scale(float)"/>
    public static Vector2 Scale(this Vector2 v)
    {
        return new Vector2(v.X.Scale(), v.Y.Scale());
    }

    /// <summary>
    ///     Scale a float value based on the two independent Dalamud UI scaling factors.
    /// </summary>
    /// <param name="f">The float value to scale.</param>
    /// <returns>The float value scaled with the user's style settings.</returns>
    public static float Scale(this float f)
    {
        return f * ImGuiHelpers.GlobalScale * (Svc.PluginInterface.UiBuilder.DefaultFontSpec.SizePt / 12f);
    }

    /// <inheritdoc cref="Scale(float)"/>
    public static float? Scale(this float? f)
    {
        return f?.Scale();
    }

    /// <inheritdoc cref="ImGuiEx.BeginDefaultTable(string, string[], bool, ImGuiTableFlags, bool)"/>
    public static bool BeginDefaultTable(string[] headers, bool drawHeader = true, ImGuiTableFlags extraFlags = ImGuiTableFlags.None)
    {
        return BeginDefaultTable("##ECommonsDefaultTable", headers, drawHeader, extraFlags);
    }

    /// <summary>
    /// Begins a typical most desired table avoiding unnecessary boilerplate with borders, changing row background and . You must still end it with <see cref="ImGui.EndTable"/>.
    /// </summary>
    /// <param name="id">ImGui ID</param>
    /// <param name="headers">An array of columns. Prefix column name with "~" to make it stretching column.</param>
    /// <param name="drawHeader">Whether to draw a header</param>
    /// <param name="extraFlags">Add extra flags to the table</param>
    /// <param name="flagsOverride">If <see langword="true"/>, <paramref name="extraFlags"/> will override <see cref="ImGuiEx.DefaultTableFlags"></see> completely</param>
    /// <returns>Same as <see cref="ImGui.BeginTable(ImU8String, int, ImGuiTableFlags, Vector2, float)"/></returns>
    public static bool BeginDefaultTable(string id, string[] headers, bool drawHeader = true, ImGuiTableFlags extraFlags = ImGuiTableFlags.None, bool flagsOverride = false)
    {
        if(ImGui.BeginTable(id, headers.Length, flagsOverride ? extraFlags : DefaultTableFlags | extraFlags))
        {
            DefaultTableColumns(headers, drawHeader);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Setups table columns.
    /// </summary>
    /// <param name="headers">An array of columns. Prefix column name with "~" to make it stretching column.</param>
    /// <param name="drawHeader">Whether to draw a header row</param>
    public static void DefaultTableColumns(IEnumerable<string> headers, bool drawHeader = true)
    {
        foreach(var x in headers)
        {
            var stretch = x.StartsWith('~');
            ImGui.TableSetupColumn(stretch ? x[1..] : x, stretch ? ImGuiTableColumnFlags.WidthStretch : ImGuiTableColumnFlags.None);
        }
        if(drawHeader) ImGui.TableHeadersRow();
    }

    /// <summary>
    /// Trims string to fit it into <see cref="ImGui.GetContentRegionAvail"/> and applies "..." if trimmed.
    /// </summary>
    /// <param name="str">String to trim</param>
    /// <param name="minTrimLength">Minimum length for string to be considered trimmable. Strings of less length will be returned as is.</param>
    /// <returns></returns>
    public static string ImGuiTrim(this string str, int minTrimLength = 5)
    {
        if(str.Length < minTrimLength) return str;
        var size = ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("...").X;
        for(var i = 1; i < str.Length; i++)
        {
            if(ImGui.CalcTextSize(str[..i]).X > size)
            {
                return str[..(i - 1)] + "...";
            }
        }
        return str;
    }

    /// <summary>
    /// Trims string and applies "..." if trimmed.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="trimToLength">To how many symbols to trim the string</param>
    /// <returns></returns>
    public static string Trim(this string str, int trimToLength)
    {
        if(str.Length > trimToLength) return str[..trimToLength] + "...";
        return str;
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

    ///<inheritdoc cref="ImGuiEx.TreeNodeCollapsingHeader(string, bool, System.Action, ImGuiTreeNodeFlags)"/>
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
            var ret = TreeNode(name, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Selected | extraFlags);
            ImGui.PopStyleVar();
            if(ret)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if(ImGui.BeginTable($"2{name}", 1, ImGuiTableFlags.NoSavedSettings | (usePadding ? ImGuiTableFlags.PadOuterX : ImGuiTableFlags.None)))
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
        prependText ??= all ? "The following plugins are required to be installed and enabled:" : "One of the following plugins is required to be installed and enabled";
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
        Text(pass ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, pass ? FontAwesomeIcon.Check.ToIconString() : "\uf00d");
        ImGui.PopFont();
        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            Text(prependText);
            ImGui.PopTextWrapPos();
            foreach(var info in pluginInfos)
            {
                var plugin = Svc.PluginInterface.InstalledPlugins.FirstOrDefault(x => x.IsLoaded && x.InternalName == info.InternalName);
                if(plugin != null)
                {
                    if(info.MinVersion == null || plugin.Version >= info.MinVersion)
                    {
                        Text(ImGuiColors.ParsedGreen, $"- {info.VanityName ?? info.InternalName} {(info.MinVersion == null ? "" : $" {info.MinVersion}+")}");
                    }
                    else
                    {
                        Text(ImGuiColors.ParsedGreen, $"- {info.VanityName ?? info.InternalName} ");
                        ImGui.SameLine(0, 0);
                        Text(ImGuiColors.DalamudRed, $"{info.MinVersion}+ ");
                        ImGui.SameLine(0, 0);
                        Text($"(outdated)");
                    }
                }
                else
                {
                    Text(ImGuiColors.DalamudRed, $"- {info.VanityName ?? info.InternalName} " + (info.MinVersion == null ? "" : $"{info.MinVersion}+ "));
                    ImGui.SameLine(0, 0);
                    Text($"(not installed)");
                }
            }
            ImGui.EndTooltip();
        }

    }

    /// <summary>
    /// Same as ImGui.Selectable, but allows it to be "disabled"
    /// </summary>
    /// <param name="color">Text color</param>
    /// <param name="id"></param>
    /// <param name="enabled"></param>
    /// <param name="selected"></param>
    /// <param name="flags"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool Selectable(Vector4? color, string id, bool enabled = true, bool selected = false, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None, Vector2 size = default)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        if(color != null) ImGui.PushStyleColor(ImGuiCol.Text, color.Value);
        var ret = ImGui.Selectable(id, selected, flags, size) && enabled;
        if(color != null) ImGui.PopStyleColor();
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    ///<inheritdoc cref="ImGuiEx.Selectable(string, bool, bool, ImGuiSelectableFlags, Vector2)"/>
    public static bool Selectable(string id, bool enabled = true, bool selected = false, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None, Vector2 size = default)
    {
        return Selectable(null, id, enabled, selected, flags, size);
    }

    /// <summary>
    /// Selectable item made from TreeNode with bullet mark in front
    /// </summary>
    /// <param name="color">Text color</param>
    /// <param name="id">ImGui ID</param>
    /// <param name="enabled">Whether node is enabled</param>
    /// <returns><see langword="true"/> when clicked</returns>
    public static bool SelectableNode(Vector4? color, string id, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        var ret = TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Leaf) && enabled;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    /// <inheritdoc cref="SelectableNode(Vector4?, string, bool)"/>
    public static bool SelectableNode(string id, bool enabled = true) => SelectableNode(null, id, enabled);

    /// <inheritdoc cref="SelectableNode(Vector4?, string, bool)"/>
    public static bool SelectableNode(string id, ref bool selected, bool enabled = true) => SelectableNode(null, id, ref selected, enabled: enabled);

    /// <summary>
    /// Selectable item made from TreeNode
    /// </summary>
    /// <param name="color">Text color</param>
    /// <param name="id">ImGui ID</param>
    /// <param name="selected">Selected state storage field</param>
    /// <param name="extraFlags">Extra tree node flags</param>
    /// <param name="enabled">Whether node is enabled</param>
    /// <returns><see langword="true"/> when clicked</returns>
    public static bool SelectableNode(Vector4? color, string id, ref bool selected, ImGuiTreeNodeFlags extraFlags = ImGuiTreeNodeFlags.Leaf, bool enabled = true)
    {
        if(!enabled) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.6f);
        TreeNode(color, id, ImGuiTreeNodeFlags.NoTreePushOnOpen | (selected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) | extraFlags);
        var ret = enabled && ImGui.IsItemClicked(ImGuiMouseButton.Left);
        if(ret) selected = !selected;
        if(!enabled) ImGui.PopStyleVar();
        return ret;
    }

    ///<inheritdoc cref="TreeNode(Vector4?, string, ImGuiTreeNodeFlags)"/>
    public static bool TreeNode(string name, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanFullWidth) => TreeNode(null, name, flags);

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
        /// <summary>
        /// Whether to add bulk job selectors
        /// </summary>
        BulkSelectors,
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
        if(ImGui.BeginCombo(id, preview, ImGuiComboFlags.HeightLarge))
        {
            if(ImGui.IsWindowAppearing() && options?.Contains(JobSelectorOption.ClearFilterOnOpen) == true)
                ImGui.SetNextItemWidth(150f);
            ImGui.InputTextWithHint("##filter", "Filter...", ref JobSelectorFilter, 50);
            if(options?.Contains(JobSelectorOption.BulkSelectors) == true)
            {
                ImGuiEx.CollectionCheckbox("DoW/DoM", Enum.GetValues<Job>().Where(x => x.IsCombat()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("DoL", Enum.GetValues<Job>().Where(x => x.IsDol()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Tanks", Enum.GetValues<Job>().Where(x => x.IsTank()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Healers", Enum.GetValues<Job>().Where(x => x.IsHealer()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("DPS", Enum.GetValues<Job>().Where(x => x.IsDps()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Melee DPS", Enum.GetValues<Job>().Where(x => x.IsMeleeDps()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Ranged DPS", Enum.GetValues<Job>().Where(x => x.IsRangedDps()), selectedJobs);
                ImGuiEx.CollectionCheckbox("Magical ranged DPS", Enum.GetValues<Job>().Where(x => x.IsMagicalRangedDps()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Physical ranged DPS", Enum.GetValues<Job>().Where(x => x.IsPhysicalRangedDps()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Ranged jobs", Enum.GetValues<Job>().Where(x => x.IsHealer() || x.IsRangedDps()), selectedJobs);
                ImGui.SameLine();
                ImGuiEx.CollectionCheckbox("Melee jobs", Enum.GetValues<Job>().Where(x => x.IsMeleeDps() || x.IsTank()), selectedJobs);
            }
            foreach(var cond in Enum.GetValues<Job>().Where(x => baseJobs || !x.IsUpgradeable()).OrderByDescending(x => Svc.Data.GetExcelSheet<ClassJob>().GetRow((uint)x).Role))
            {
                if(cond == Job.ADV) continue;
                if(jobDisplayFilter != null && !jobDisplayFilter(cond)) continue;
                var name = cond.ToString().Replace("_", " ");
                if(JobSelectorFilter == "" || name.Contains(JobSelectorFilter, StringComparison.OrdinalIgnoreCase))
                {
                    if(ThreadLoadImageHandler.TryGetIconTextureWrap((uint)cond.GetIcon(), false, out var texture))
                    {
                        ImGui.Image(texture.Handle, new Vector2(24f.Scale()));
                        ImGui.SameLine();
                    }
                    if(CollectionCheckbox(name, cond, selectedJobs)) ret = true;
                }
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    ///<inheritdoc cref="ImGuiEx.InfoMarker(string, Vector4?, string, bool, bool)"/>
    public static void HelpMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true, bool preserveCursor = false) => InfoMarker(helpText, color, symbolOverride, sameLine, preserveCursor);

    /// <summary>
    /// <see cref="ImGuiComponents.HelpMarker(string)"/> but with more options
    /// </summary>
    /// <param name="helpText"></param>
    /// <param name="color"></param>
    /// <param name="symbolOverride"></param>
    /// <param name="sameLine">Whether to call SameLine before drawing marker</param>
    public static void InfoMarker(string helpText, Vector4? color = null, string symbolOverride = null, bool sameLine = true, bool preserveCursor = false)
    {
        if(preserveCursor && sameLine) ImGui.SameLine(0, 0);
        else if(sameLine) ImGui.SameLine();
        var cursor = ImGui.GetCursorPos();
        ImGui.PushFont(UiBuilder.IconFont);
        if(preserveCursor)
        {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - ImGui.CalcTextSize(symbolOverride ?? FontAwesomeIcon.InfoCircle.ToIconString()).X);
        }
        Text(color ?? ImGuiColors.DalamudGrey3, symbolOverride ?? FontAwesomeIcon.InfoCircle.ToIconString());
        ImGui.PopFont();
        if(preserveCursor)
        {
            ImGui.SetCursorPos(cursor);
        }
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
        var lastSpacePos = -1;
        for(var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            currentWidth += ImGui.CalcTextSize(c.ToString()).X;
            if(char.IsWhiteSpace(c))
            {
                lastSpacePos = i;
            }
            if(currentWidth > wrapWidth)
            {
                return lastSpacePos >= 0 ? lastSpacePos : i;
            }
        }
        return -1;
    }

    private static unsafe int TextEditCallback(ref ImGuiInputTextCallbackData dataRef, float wrapWidth)
    {
        fixed(ImGuiInputTextCallbackData* data = &dataRef)
        {
            var text = Marshal.PtrToStringAnsi((IntPtr)data->Buf, data->BufTextLen);
            var lines = text.Split('\n').ToList();
            var textModified = false;
            // Traverse each line to check if it exceeds the wrap width
            for(var i = 0; i < lines.Count; i++)
            {
                var lineWidth = ImGui.CalcTextSize(lines[i]).X;
                while(lineWidth + 10f > wrapWidth)
                {
                    // Find where to break the line
                    var wrapPos = FindWrapPosition(lines[i], wrapWidth);
                    if(wrapPos >= 0)
                    {
                        // Insert a newline at the wrap position
                        var part1 = lines[i].Substring(0, wrapPos);
                        var part2 = lines[i].Substring(wrapPos).TrimStart();
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
            if(textModified)
            {
                var newText = string.Join("\n", lines);
                var newTextBytes = Encoding.UTF8.GetBytes(newText.PadRight(data->BufSize, '\0'));
                Marshal.Copy(newTextBytes, 0, (IntPtr)data->Buf, newTextBytes.Length);
                data->BufTextLen = newText.Length;
                data->BufDirty = 1;
                data->CursorPos = Math.Min(data->CursorPos, data->BufTextLen);
            }
            return 0;
        }
    }

    [Obsolete($"Use RealtimeDragDrop. Better user experience anyway.")]
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
            if(ImGuiEx.IconButton(FontAwesomeIcon.ArrowUp) && i > 0)
            {
                (order[i - 1], order[i]) = (order[i], order[i - 1]);
                ret = true;
            }
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.AngleDown) && i < order.Count - 1)
            {
                (order[i + 1], order[i]) = (order[i], order[i + 1]);
                ret = true;
            }
            ImGui.SameLine();
            Text($"{e.ToString().Replace("_", " ")}");
            ImGui.PopID();
        }
        return ret;
    }

    /// <summary>
    /// Checks whether item is hovered and clicked. Sets cursor to hand to indicate that it can be clicked.
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

    /// <summary>
    /// Just an ImGui.CollapsingHeader with convenient color selection
    /// </summary>
    /// <param name="col"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool CollapsingHeader(Vector4? col, string text)
    {
        if(col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        var ret = ImGui.CollapsingHeader(text);
        if(col != null) ImGui.PopStyleColor();
        return ret;
    }

    ///<inheritdoc cref="ImGuiEx.CollapsingHeader(Vector4?, string)"/>
    public static bool CollapsingHeader(string text)
    {
        return CollapsingHeader(null, text);
    }

    [Obsolete("Move color parameter to be the first one")]
    public static bool CollapsingHeader(string text, Vector4? col)
    {
        if(col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        var ret = ImGui.CollapsingHeader(text);
        if(col != null) ImGui.PopStyleColor();
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

    public static bool BeginPopupNextToElement(string popupId)
    {
        ImGui.SameLine(0, 0);
        var pos = ImGui.GetCursorScreenPos();
        ImGui.Dummy(Vector2.Zero);
        ImGui.SetNextWindowPos(pos, ImGuiCond.Appearing);
        return ImGui.BeginPopup(popupId);
    }

    public static Vector4 MutateColor(ImGuiCol col, byte r, byte g, byte b)
    {
        return ImGui.GetStyle().Colors[(int)col] with { X = (float)r / 255f, Y = (float)g / 255f, Z = (float)b / 255f };
    }

    public static bool IsKeyPressed(int key, bool repeat)
    {
        return ImGui.IsKeyPressed((ImGuiKey)key, repeat);
    }

    public static float GetWindowContentRegionWidth()
    {
        return ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    }

    public static void Spacing() => Spacing(null);

    [Obsolete("Use ImGuiEx.Spacing(Vector2, bool) instead")]
    public static void Spacing(float pix = 10f, bool accountForScale = true)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (accountForScale ? pix : pix.Scale()));
    }

    public static void Spacing(Vector2? size = null, bool accountForScale = true)
    {
        size ??= new Vector2(10f);

        if(accountForScale)
            size = size.Scale();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + size!.Value.X);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + size!.Value.Y);
    }

    public static void SetTooltip(string text)
    {
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
        ImGui.TextUnformatted(text);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }

    /// <summary>
    /// Sets next item width to <see cref="ImGui.GetContentRegionAvail"/>
    /// </summary>
    /// <param name="mod">Extra pixels to add</param>
    public static void SetNextItemFullWidth(int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X + mod);
    }

    /// <summary>
    /// Sets next item width to a certain percentage of <see cref="ImGui.GetContentRegionAvail"/> 
    /// </summary>
    /// <param name="percent">How much in % to set width to</param>
    /// <param name="mod">Extra pixels to add</param>
    public static void SetNextItemWidth(float percent, int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X * percent + mod);
    }

    [Obsolete("Just push style col")]
    public static void WithTextColor(Vector4 col, Action func)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        GenericHelpers.Safe(func);
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Displays tooltip if the item is hovered
    /// </summary>
    /// <param name="s"></param>
    public static void Tooltip(string s)
    {
        if(ImGui.IsItemHovered())
        {
            SetTooltip(s);
        }
    }

    [Obsolete("Probably isn't used by anyone")]
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

    public static void EzTabBar(string id, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, null, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, params (string name, Action function, Vector4? color, bool child)[] tabs) => EzTabBar(id, KoFiTransparent, openTabName, ImGuiTabBarFlags.None, tabs);
    public static void EzTabBar(string id, string KoFiTransparent, string openTabName, ImGuiTabBarFlags flags, params (string name, Action function, Vector4? color, bool child)[] tabs)
    {
        if(ImGui.BeginTabBar(id, flags))
        {
            foreach(var x in tabs)
            {
                if(x.name == null) continue;
                if(x.color != null)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, x.color.Value);
                }
                if(ImGui.BeginTabItem(x.name, openTabName == x.name ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None))
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
    }

    public static bool Ctrl => ImGui.GetIO().KeyCtrl;
    public static bool Alt => ImGui.GetIO().KeyAlt;
    public static bool Shift => ImGui.GetIO().KeyShift;

    public static Vector2 CalcIconSize(string icon, bool isButton = false)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.CalcTextSize($"{icon}");
        ImGui.PopFont();
        return result + (isButton ? ImGui.GetStyle().FramePadding * 2f : Vector2.Zero);
    }

    public static Vector2 CalcIconSize(FontAwesomeIcon icon, bool isButton = false)
    {
        return CalcIconSize(icon.ToIconString(), isButton);
    }

    /// <summary>
    /// Records initial cursor positions and measures width difference after executing a function.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="includeSpacing">Whether to include <see cref="ImGui.GetStyle"/>.ItemSpacing</param>
    /// <returns></returns>
    public static float Measure(Action func, bool includeSpacing = true)
    {
        var pos = ImGui.GetCursorPosX();
        func();
        ImGui.SameLine(0, 0);
        var diff = ImGui.GetCursorPosX() - pos;
        ImGui.NewLine();
        return diff + (includeSpacing ? ImGui.GetStyle().ItemSpacing.X : 0);
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

    [Obsolete("Switch to using ImGui.BeginTabItem. It now has version without \"close\".", true)]
    public static unsafe bool BeginTabItem(string label, ImGuiTabItemFlags flags)
    {
        throw new NotImplementedException("Switch to using ImGui.BeginTabItem. It now has version without \"close\".");
    }

    [Obsolete("Use Marshal.AllocHGlobal", true)]
    internal static unsafe byte* Allocate(int byteCount)
    {
        return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
    }

    [Obsolete("Use Marshal.FreeHGlobal", true)]
    internal static unsafe void Free(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    [Obsolete("Use Encoding.UTF8.GetBytes", true)]
    internal static unsafe int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed(char* chars = s)
        {
            return Encoding.UTF8.GetBytes(chars, s.Length, utf8Bytes, utf8ByteCount);
        }
    }
}

