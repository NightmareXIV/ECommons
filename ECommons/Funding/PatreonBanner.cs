using Dalamud.Interface.Utility;
using ECommons.EzSharedDataManager;
using ECommons.ImGuiMethods;
using ECommons.Resources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Funding;
public static class PatreonBanner
{
		public static Func<bool> IsOfficialPlugin = () => false;
		public static string Text = Lang.JoinPatreon;
		public static string DonateLink => "https://www.patreon.com/NightmareXIV";
		public static void DrawRaw()
		{
				DrawButton();
		}

		static uint ColorNormal => EzSharedData.GetOrCreate<uint[]>("ECommonsPatreonBannerRandomColor", [GradientColor.Get(ImGuiEx.Vector4FromRGB(0x022594), ImGuiEx.Vector4FromRGB(0x940238)).ToUint()])[0];
    static uint ColorHovered => ColorNormal;
    static uint ColorActive => ColorNormal;
    static readonly uint ColorText = 0xFFFFFFFF;

		public static void DrawButton()
		{
				ImGui.PushStyleColor(ImGuiCol.Button, ColorNormal);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHovered);
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorActive);
				ImGui.PushStyleColor(ImGuiCol.Text, ColorText);
				if (ImGui.Button(Text))
				{
						GenericHelpers.ShellStart(DonateLink);
				}
				Popup();
				if (ImGui.IsItemHovered())
				{
						ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
				}
				ImGui.PopStyleColor(4);
		}

		public static void RightTransparentTab()
		{
				var textWidth = ImGui.CalcTextSize(KoFiButton.Text).X;
				var spaceWidth = ImGui.CalcTextSize(" ").X;
				ImGui.BeginDisabled();
				ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0f);
				if (ImGuiEx.BeginTabItem(" ".Repeat((int)MathF.Ceiling(textWidth / spaceWidth)), ImGuiTabItemFlags.Trailing))
				{
						ImGui.EndTabItem();
				}
				ImGui.PopStyleVar();
				ImGui.EndDisabled();
		}

		public static void DrawRight()
		{
				var cur = ImGui.GetCursorPos();
				ImGui.SetCursorPosX(cur.X + ImGui.GetContentRegionAvail().X - ImGuiHelpers.GetButtonSize(Text).X);
				DrawRaw();
				ImGui.SetCursorPos(cur);
		}

    private static void Popup()
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGuiEx.Text(IsOfficialPlugin()?Lang.SmallPatreonButtonTooltip:Lang.PatreonButtonTooltip);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("NXPS");
            }
        }
        if (ImGui.BeginPopup("NXPS"))
        {
            if (ImGui.Selectable(Lang.Context_Subscribe))
            {
                GenericHelpers.ShellStart("https://subscribe.nightmarexiv.com");
            }
            if (ImGui.Selectable(Lang.Context_OneTimeDonate))
            {
                GenericHelpers.ShellStart("https://donate.nightmarexiv.com");
            }
            if (ImGui.Selectable(Lang.Context_CryptoDonation))
            {
                GenericHelpers.ShellStart("https://crypto.nightmarexiv.com");
            }
						if (!IsOfficialPlugin())
						{
								if (ImGui.Selectable(Lang.Context_JoinDiscord))
								{
										GenericHelpers.ShellStart("https://discord.nightmarexiv.com");
								}
								if (ImGui.Selectable(Lang.Context_ExploreOtherPlugins))
								{
										GenericHelpers.ShellStart("https://explore.nightmarexiv.com");
								}
						}
						ImGui.EndPopup();
        }
    }
}
