using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using ECommons.DalamudServices;
using ImGuiNET;

namespace ECommons.ImGuiMethods
{
    public static class ImGuiEx
    {
        public static uint ToUint(this Vector4 color) 
        {
            return ImGui.ColorConvertFloat4ToU32(color);
        }
        
        public static void InvisibleButton(int width = 0)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
            ImGui.Button(" ");
            ImGui.PopStyleColor(3);
        }

        public static void EnumCombo<T>(string name, ref T refConfigField, string[] overrideNames = null) where T : IConvertible
        {
            var values = overrideNames ?? Enum.GetValues(typeof(T)).Cast<T>().Select(x => x.ToString().Replace("_", " ")).ToArray();
            var num = Convert.ToInt32(refConfigField);
            ImGui.Combo(name, ref num, values, values.Length);
            refConfigField = Enum.GetValues(typeof(T)).Cast<T>().ToArray()[num];
        }

        public static bool IconButton(FontAwesomeIcon icon, string id = "ECommonsButton")
        {
            ImGui.PushFont(UiBuilder.IconFont);
            var result = ImGui.Button($"{icon.ToIconString()}##{icon.ToIconString()}-{id}");
            ImGui.PopFont();
            return result;
        }

        public static float Measure(Action func)
        {
            var pos = ImGui.GetCursorPosX();
            func();
            ImGui.SameLine(0, 0);
            var diff = ImGui.GetCursorPosX() - pos;
            ImGui.Dummy(Vector2.Zero);
            return diff;
        }

        public static void InputHex(string name, ref uint hexInt)
        {
            var text = $"{hexInt:X}";
            if (ImGui.InputText(name, ref text, 8))
            {
                if (uint.TryParse(text, NumberStyles.HexNumber, null, out var num))
                {
                    hexInt = num;
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

        public static void TextCopy(string text)
        {
            ImGui.TextUnformatted(text);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                ImGui.SetClipboardText(text);
                Svc.PluginInterface.UiBuilder.AddNotification("Text copied to clipboard", null, NotificationType.Success);
            }
        }

        public static void ButtonCopy(string buttonText, string copy)
        {
            if(ImGui.Button(buttonText.Replace("$COPY", copy)))
            {
                ImGui.SetClipboardText(copy);
                Svc.PluginInterface.UiBuilder.AddNotification("Text copied to clipboard", null, NotificationType.Success);
            }
        }
    }
}
