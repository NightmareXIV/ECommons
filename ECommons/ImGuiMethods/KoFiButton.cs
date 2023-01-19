using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public static class KoFiButton
    {
        public static bool IsOfficialPlugin = false;
        internal const string Text = "Donate (Ko-fi)";
        public static void DrawRaw()
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudOrange);
            if(ImGui.Button(Text))
            {
                GenericHelpers.ShellStart("https://nightmarexiv.github.io/donate.html" + (IsOfficialPlugin ? "?official" : ""));
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }
            ImGui.PopStyleColor();
        }

        public static void DrawRight()
        {
            var cur = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(cur.X + ImGui.GetContentRegionAvail().X - ImGuiHelpers.GetButtonSize(Text).X);
            DrawRaw();
            ImGui.SetCursorPos(cur);
        }
    }
}
