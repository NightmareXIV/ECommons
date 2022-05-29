using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal.Notifications;
using ECommons.DalamudServices;
using ImGuiNET;

namespace ECommons.ImGuiMethods
{
    public static class ImGuiEx
    {
        public static void Text(string s)
        {
            ImGui.TextUnformatted(s);
        }

        public static void Text(Vector4 col, string s)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, col);
            ImGui.TextUnformatted(s);
            ImGui.PopStyleColor();
        }

        public static void Text(uint col, string s)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, col);
            ImGui.TextUnformatted(s);
            ImGui.PopStyleColor();
        }

        public static void TextWrapped(string s)
        {
            ImGui.PushTextWrapPos();
            ImGui.TextUnformatted(s);
            ImGui.PopTextWrapPos();
        }

        public static void TextWrapped(Vector4 col, string s)
        {
            ImGui.PushTextWrapPos();
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

        public static void EzTabBar(string id, params (string name, Action function, Vector4? color, bool child)[] tabs)
        {
            ImGui.BeginTabBar(id);
            foreach(var x in tabs)
            {
                if(x.color != null)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, x.color.Value);
                }
                if (ImGui.BeginTabItem(x.name))
                {
                    if (x.color != null)
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
                    if (x.color != null)
                    {
                        ImGui.PopStyleColor();
                    }
                }
            }
            ImGui.EndTabBar();
        }

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
