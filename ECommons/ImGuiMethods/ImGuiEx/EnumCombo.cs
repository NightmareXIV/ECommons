using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ECommons.ImGuiMethods;

public static partial class ImGuiEx
{
    public static Dictionary<string, Box<string>> EnumComboSearch = [];

    /// <summary>
    /// Draws an easy combo selector for an enum with a search field for long lists.
    /// </summary>
    /// <typeparam name="T">Enum</typeparam>
    /// <param name="name">ImGui ID</param>
    /// <param name="refConfigField">Value</param>
    /// <param name="names">Optional Name overrides</param>
    public static bool EnumCombo<T>(string name, ref T refConfigField, IDictionary<T, string> names, T[]? valuesList = null) where T : Enum, IConvertible
    {
        return EnumCombo(name, ref refConfigField, null, names, valuesList: valuesList);
    }

    public static bool EnumCombo<T>(float width, string name, ref T refConfigField, Func<T, bool> filter = null, IDictionary<T, string> names = null, T[]? valuesList = null) where T : Enum, IConvertible
    {
        ImGui.SetNextItemWidth(width);
        return EnumCombo(name, ref refConfigField, filter, names, valuesList);
    }

    [OverloadResolutionPriority(1)]
    /// <summary>
    /// Draws an easy combo selector for an enum with a search field for long lists.
    /// </summary>
    /// <typeparam name="T">Enum</typeparam>
    /// <param name="name">ImGui ID</param>
    /// <param name="refConfigField">Value</param>
    /// <param name="filter">Optional filter</param>
    /// <param name="names">Optional Name overrides</param>
    /// <returns></returns>
    public static bool EnumCombo<T>(string name, ref T refConfigField, Func<T, bool> filter = null, IDictionary<T, string> names = null, T[]? valuesList = null) where T : Enum, IConvertible
    {
        var ret = false;
        if(ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString().Replace("_", " "), ImGuiComboFlags.HeightLarge))
        {
            var values = valuesList as Array ?? Enum.GetValues(typeof(T));
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


    public static bool EnumCombo<T>(string name, ICollection<T> collection, Func<T, bool> filter = null, IDictionary<T, string> names = null, string nameOfNoneSelected = "Not selected", T[]? valuesList = null) where T : Enum, IConvertible
    {
        var ret = false;
        string comboName;
        if(collection.Count == 0)
        {
            comboName = nameOfNoneSelected;
        }
        else
        {
            comboName = collection.Select(x => (names != null && names.TryGetValue(x, out var n)) ? n : x.ToString().Replace("_", " ")).Print();
        }
        if(ImGui.BeginCombo(name, comboName, ImGuiComboFlags.HeightLarge))
        {
            var values = valuesList as Array ?? Enum.GetValues(typeof(T));
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
                var contains = collection.Any(c => EqualityComparer<T>.Default.Equals((T)x, c));
                var element = (names != null && names.TryGetValue((T)x, out var n)) ? n : x.ToString().Replace("_", " ");
                if((filter == null || filter((T)x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Checkbox(element, ref contains)
                    )
                {
                    ret = true;
                    if(contains)
                    {
                        collection.Add((T)x);
                    }
                    else
                    {
                        collection.Remove((T)x);
                    }
                }
            }
            foreach(var x in collection.Where(z => !Enum.IsDefined(typeof(T), z)))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                var contains = collection.Any(c => EqualityComparer<T>.Default.Equals((T)x, c));
                var element = (names != null && names.TryGetValue((T)x, out var n)) ? n : x.ToString().Replace("_", " ");
                if((filter == null || filter((T)x))
                    && (fltr == null || element.Contains(fltr.Value, StringComparison.OrdinalIgnoreCase))
                    && ImGui.Checkbox(element, ref contains)
                    )
                {
                    ret = true;
                    if(contains)
                    {
                        collection.Add((T)x);
                    }
                    else
                    {
                        collection.Remove((T)x);
                    }
                }
                ImGui.PopStyleColor();
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    public static bool EnumCombo<T>(string name, ref Nullable<T> refConfigField, Func<T, bool> filter = null, IDictionary<T, string> names = null, string nullName = "Not selected", T[]? valuesList = null) where T : struct, Enum, IConvertible
    {
        var ret = false;
        if(ImGui.BeginCombo(name, refConfigField == null ? nullName : ((names != null && names.TryGetValue(refConfigField.Value, out var n)) ? n : refConfigField.Value.ToString().Replace("_", " ")), ImGuiComboFlags.HeightLarge))
        {
            var values = valuesList as Array ?? Enum.GetValues(typeof(T));
            Box<string> fltr = null;
            if(values.Length > 10)
            {
                if(!EnumComboSearch.ContainsKey(name)) EnumComboSearch.Add(name, new(""));
                fltr = EnumComboSearch[name];
                ImGuiEx.SetNextItemFullWidth();
                ImGui.InputTextWithHint($"##{name.Replace("#", "_")}", "Filter...", ref fltr.Value, 50);
            }
            if(ImGui.Selectable(nullName, refConfigField == null))
            {
                ret = true;
                refConfigField = null;
            }
            foreach(var x in values)
            {
                var equals = EqualityComparer<Nullable<T>>.Default.Equals((T)x, refConfigField);
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
}
