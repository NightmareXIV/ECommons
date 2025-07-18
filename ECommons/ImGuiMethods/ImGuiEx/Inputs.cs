using Dalamud.Interface.Utility;
using ECommons.Throttlers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ECommons.ImGuiMethods;
public static partial class ImGuiEx
{
    public static bool InputFancyNumeric(string label, ref int number, int step)
    {
        var str = $"{number:N0}";
        var lbl = label.StartsWith("##") ? label : $"##{label}";
        var ret = ImGui.InputText(lbl, ref str, 50, ImGuiInputTextFlags.AutoSelectAll);
        var btn = false;
        if(step > 0)
        {
            ImGui.SameLine(0, 1);
            if(ImGui.Button($"-##minus{label}", new(ImGui.GetFrameHeight())))
            {
                ret = true;
                number -= step;
                btn = true;
            }
            if(ImGui.IsItemHovered() && ImGui.GetIO().MouseDownDuration[0] > 0.5f && EzThrottler.Throttle("FancyInputHold", 50))
            {
                ret = true;
                number -= step;
                btn = true;
            }
            ImGui.SameLine(0, 1);
            if(ImGui.Button($"+##plus{label}", new(ImGui.GetFrameHeight())))
            {
                ret = true;
                number += step;
                btn = true;
            }
            if(ImGui.IsItemHovered() && ImGui.GetIO().MouseDownDuration[0] > 0.5f && EzThrottler.Throttle("FancyInputHold", 50))
            {
                ret = true;
                number += step;
                btn = true;
            }
        }
        if(ret && !btn)
        {
            var mult = 1;
            str = str.Trim();
            if(str == "")
            {
                number = 0;
            }
            else
            {
                var negative = str[0] == '-';
                if(negative)
                {
                    str = str[1..];
                }
                while(str.EndsWith("M", StringComparison.OrdinalIgnoreCase))
                {
                    mult *= 1000000;
                    str = str[0..^1];
                }
                while(str.EndsWith("K", StringComparison.OrdinalIgnoreCase))
                {
                    mult *= 1000;
                    str = str[0..^1];
                }
                if(double.TryParse(str, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, null, out var dresult))
                {
                    number = (int)(dresult * (double)mult);
                    if(negative) number *= -1;
                }
            }
        }
        if(!label.StartsWith("##"))
        {
            ImGui.SameLine();
            ImGuiEx.Text(label);
        }
        return ret || btn;
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
            valueNullable = enabled ? 0 : null;
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

    public static unsafe bool InputTextWrapMultilineExpanding(string id, ref string text, uint maxLength = 500, int minLines = 2, int maxLines = 10, int? width = null)
    {
        var wrapWidth = width ?? ImGui.GetContentRegionAvail().X; // determine wrap width
        var result = ImGui.InputTextMultiline(id, ref text, maxLength,
            new(width ?? ImGui.GetContentRegionAvail().X, ImGui.CalcTextSize("A").Y * Math.Clamp(text.Split("\n").Length + 1, minLines, maxLines)),
            ImGuiInputTextFlags.CallbackEdit, // flag stuff 
            (data) =>
            {
                return TextEditCallback(data, wrapWidth); // Callback Action
            });
        return result;
    }

    public static bool InputTextMultilineExpanding(string id, ref string text, uint maxLength = 500, int minLines = 2, int maxLines = 10, int? width = null)
    {
        return ImGui.InputTextMultiline(id, ref text, maxLength, new(width ?? ImGui.GetContentRegionAvail().X, ImGui.CalcTextSize("A").Y * Math.Clamp(text.Split("\n").Length + 1, minLines, maxLines)));
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

    public static bool InputLong(string id, ref long num)
    {
        var txt = num.ToString();
        var ret = ImGui.InputText(id, ref txt, 50);
        long.TryParse(txt, out num);
        return ret;
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
        if(ImGui.BeginCombo(name, (names != null && names.TryGetValue(refConfigField, out var n)) ? n : refConfigField.ToString().Replace("_", " "), ImGuiComboFlags.HeightLarge))
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

    public static bool EnumCombo<T>(string name, ref Nullable<T> refConfigField, Func<T, bool> filter = null, IDictionary<T, string> names = null, string nullName = "Not selected") where T : struct, Enum, IConvertible
    {
        var ret = false;
        if(ImGui.BeginCombo(name, refConfigField == null?nullName:((names != null && names.TryGetValue(refConfigField.Value, out var n)) ? n : refConfigField.Value.ToString().Replace("_", " ")), ImGuiComboFlags.HeightLarge))
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
            if((filter == null || filter((T)x)) && ImGui.RadioButton(element, equals))
            {
                ret = true;
                refConfigField = (T)x;
            }
        }
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
    /// <param name="inverted">Whether to switch positions of <see langword="true"/> and <see langword="false"/> options</param>
    public static void RadioButtonBool(string labelTrue, string labelFalse, ref bool value, bool sameLine = false, Action prefix = null, Action suffix = null, bool inverted = false)
    {
        prefix?.Invoke();
        if(ImGui.RadioButton(inverted ? labelFalse : labelTrue, value == !inverted)) value = !inverted;
        suffix?.Invoke();
        if(sameLine) ImGui.SameLine();
        prefix?.Invoke();
        if(ImGui.RadioButton(inverted?labelTrue:labelFalse, value == inverted)) value = inverted;
        suffix?.Invoke();
    }
}
