using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ECommons.ImGuiMethods;
public static partial class ImGuiEx
{
    // from koenari https://github.com/Koenari/HimbeertoniRaidTool/blob/b28313e6d62de940acc073f203e3032e846bfb13/HimbeertoniRaidTool/UI/ImGuiHelper.cs#L188

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, t => t.ToString() ?? string.Empty, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="searchPredicate">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, string, bool> searchPredicate, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, t => t.ToString() ?? string.Empty, searchPredicate, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="preFilter"></param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, bool> preFilter, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, t => t.ToString() ?? string.Empty, preFilter, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="searchPredicate">Secondary filter to apply to the sheet for which items to display.</param>
    /// <param name="preFilter">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, string, bool> searchPredicate, Func<T, bool> preFilter, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, t => t.ToString() ?? string.Empty, searchPredicate, preFilter, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="toName">The format of each item in the combo box.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, string> toName, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, toName, (t, s) => toName(t).Contains(s, StringComparison.CurrentCultureIgnoreCase), flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="toName">The format of each item in the combo box.</param>
    /// <param name="searchPredicate">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, string> toName, Func<T, string, bool> searchPredicate, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, toName, searchPredicate, _ => true, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="toName">The format of each item in the combo box.</param>
    /// <param name="preFilter">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected, Func<ExcelSheet<T>, string> getPreview, Func<T, string> toName, Func<T, bool> preFilter, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
        => ExcelSheetCombo(id, out selected, getPreview, toName, (t, s) => toName(t).Contains(s, StringComparison.CurrentCultureIgnoreCase), preFilter, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="getPreview">The format of the initial value in the combo box.</param>
    /// <param name="toName">The format of the initial value in the combo box.</param>
    /// <param name="searchPredicate">Secondary filter to apply to the sheet for which items to display.</param>
    /// <param name="preFilter">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool ExcelSheetCombo<T>(string id, [NotNullWhen(true)] out T selected,
        Func<ExcelSheet<T>, string> getPreview, Func<T, string> toName,
        Func<T, string, bool> searchPredicate,
        Func<T, bool> preFilter, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : struct, IExcelRow<T>
    {
        var sheet = Svc.Data.GetExcelSheet<T>();
        if(sheet is null)
        {
            selected = default;
            return false;
        }
        return SearchableCombo(id, out selected, getPreview(sheet), sheet, toName, searchPredicate, preFilter, flags);
    }

    private static string _search = string.Empty;
    private static HashSet<object>? _filtered;
    private static int _hoveredItem;
    private static readonly Dictionary<string, (bool toogle, bool wasEnterClickedLastTime)> _comboDic = [];

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="preview">The format of the initial value in the combo box.</param>
    /// <param name="possibilities">The initial excel sheet.</param>
    /// <param name="toName">The format of the initial value in the combo box.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool SearchableCombo<T>(string id, [NotNullWhen(true)] out T? selected, string preview, IEnumerable<T> possibilities, Func<T, string> toName, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : notnull
        => SearchableCombo(id, out selected, preview, possibilities, toName, (p, s) => toName.Invoke(p).Contains(s, StringComparison.InvariantCultureIgnoreCase), flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="preview">The format of the initial value in the combo box.</param>
    /// <param name="possibilities">The initial excel sheet.</param>
    /// <param name="toName">The format of each item in the combo box.</param>
    /// <param name="searchPredicate">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool SearchableCombo<T>(string id, [NotNullWhen(true)] out T? selected, string preview, IEnumerable<T> possibilities, Func<T, string> toName, Func<T, string, bool> searchPredicate, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : notnull
        => SearchableCombo(id, out selected, preview, possibilities, toName, searchPredicate, _ => true, flags);

    /// <summary>
    /// Creates a searchable combo box for a given Excel sheet with an optional custom filter.
    /// </summary>
    /// <typeparam name="T">ExcelSheet</typeparam>
    /// <param name="id">ID of the combo box.</param>
    /// <param name="selected">Excel row returned when selected.</param>
    /// <param name="preview">The format of the initial value in the combo box.</param>
    /// <param name="possibilities">The initial excel sheet.</param>
    /// <param name="toName">The format of each item in the combo box.</param>
    /// <param name="searchPredicate">Secondary filter to apply to the sheet for which items to display.</param>
    /// <param name="preFilter">Initial filter to apply to the sheet for which items to display.</param>
    /// <param name="flags">Any ImGuiComboFlags</param>
    /// <returns>Bool when item is selected.</returns>
    public static bool SearchableCombo<T>(string id, [NotNullWhen(true)] out T? selected, string preview, IEnumerable<T> possibilities, Func<T, string> toName, Func<T, string, bool> searchPredicate, Func<T, bool> preFilter, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : notnull
    {
        _comboDic.TryAdd(id, (false, false));
        (var toggle, var wasEnterClickedLastTime) = _comboDic[id];
        selected = default;
        if(!ImGui.BeginCombo(id + (toggle ? "##x" : ""), preview, flags)) return false;

        if(wasEnterClickedLastTime || ImGui.IsKeyPressed(ImGuiKey.Escape))
        {
            toggle = !toggle;
            _search = string.Empty;
            _filtered = null;
        }
        var enterClicked = ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter);
        wasEnterClickedLastTime = enterClicked;
        _comboDic[id] = (toggle, wasEnterClickedLastTime);
        if(ImGui.IsKeyPressed(ImGuiKey.UpArrow))
            _hoveredItem--;
        if(ImGui.IsKeyPressed(ImGuiKey.DownArrow))
            _hoveredItem++;
        _hoveredItem = Math.Clamp(_hoveredItem, 0, Math.Max(_filtered?.Count - 1 ?? 0, 0));
        if(ImGui.IsWindowAppearing() && ImGui.IsWindowFocused() && !ImGui.IsAnyItemActive())
        {
            _search = string.Empty;
            _filtered = null;
            ImGui.SetKeyboardFocusHere(0);
        }

        if(ImGui.InputText("##ExcelSheetComboSearch", ref _search, 128))
            _filtered = null;

        if(_filtered == null)
        {
            _filtered = possibilities.Where(preFilter).Where(s => searchPredicate(s, _search)).Cast<object>().ToHashSet();
            _hoveredItem = 0;
        }

        var i = 0;
        foreach(var row in _filtered.Cast<T>())
        {
            var hovered = _hoveredItem == i;
            ImGui.PushID(i);
            if(ImGui.Selectable(toName(row), hovered) || (enterClicked && hovered))
            {
                selected = row;
                ImGui.PopID();
                ImGui.EndCombo();
                return true;
            }
            ImGui.PopID();
            i++;
        }

        ImGui.EndCombo();
        return false;
    }
}