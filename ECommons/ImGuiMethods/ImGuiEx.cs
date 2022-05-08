using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;

namespace ECommons.ImGuiMethods
{
    public static class ImGuiEx
    {
        public static void ImGuiEnumCombo<T>(string name, ref T refConfigField, string[] overrideNames = null) where T : IConvertible
        {
            var values = overrideNames ?? Enum.GetValues(typeof(T)).Cast<T>().Select(x => x.ToString().Replace("_", " ")).ToArray();
            var num = Convert.ToInt32(refConfigField);
            ImGui.Combo(name, ref num, values, values.Length);
            refConfigField = Enum.GetValues(typeof(T)).Cast<T>().ToArray()[num];
        }

        public static bool ImGuiIconButton(FontAwesomeIcon icon, string id = "ECommonsButton")
        {
            ImGui.PushFont(UiBuilder.IconFont);
            var result = ImGui.Button($"{icon.ToIconString()}##{icon.ToIconString()}-{id}");
            ImGui.PopFont();
            return result;
        }
    }
}
