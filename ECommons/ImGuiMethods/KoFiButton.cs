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
        public const string Text = "Support on Ko-fi";
        public static string DonateLink => "https://nightmarexiv.github.io/donate.html" + (IsOfficialPlugin ? "?official" : "");
        public static void DrawRaw()
        {
            DrawButton();
        }

        public static void DrawButton()
        {
            ImGui.PushStyleColor(ImGuiCol.Button, 0xFF942502);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFF942502);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xFF942502);
            ImGui.PushStyleColor(ImGuiCol.Text, 0xFFFFFFFF);
            if (ImGui.Button(Text))
            {
                GenericHelpers.ShellStart(DonateLink);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }
            ImGui.PopStyleColor(4);
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
