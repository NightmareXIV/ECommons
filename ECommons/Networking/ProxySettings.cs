using ECommons.ImGuiMethods;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Net.Http;

namespace ECommons.Networking;
[Serializable]
public class ProxySettings
{
    [JsonProperty("UseProxy")] public bool UseProxy = false;
    [JsonProperty("ProxyAddress")] public string ProxyAddress = "";
    [JsonProperty("UseProxyAuthentication")] public bool UseProxyAuthentication = false;
    [JsonProperty("BypassLocal")] public bool BypassLocal = true;
    [JsonProperty("ProxyLogin")] public string ProxyLogin = "";
    [JsonProperty("ProxyPassword")] public string ProxyPassword = "";

    /// <summary>
    /// Draw a compact collapsing header containing proxy settings.
    /// </summary>
    public void ImGuiDraw()
    {
        ImGuiEx.TreeNodeCollapsingHeader("Proxy Settings", ImGuiDrawNoCollapsingHeader);
    }

    /// <summary>
    /// Draw proxy settings.
    /// </summary>
    public void ImGuiDrawNoCollapsingHeader()
    {
        if(ImGuiEx.BeginDefaultTable("##proxySettings", ["a", "~b"], false, ImGuiEx.DefaultTableFlags & ~ImGuiTableFlags.BordersOuter, true))
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Enable proxy:");
            ImGui.TableNextColumn();
            ImGui.Checkbox("##enableProxy", ref UseProxy);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Bypass for local connections:");
            ImGui.TableNextColumn();
            ImGui.Checkbox("##bypassLocalConnections", ref BypassLocal);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Proxy address:");
            ImGui.TableNextColumn();
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputText("##proxyAddress", ref ProxyAddress, 1000);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Enable authentication:");
            ImGui.TableNextColumn();
            ImGui.Checkbox("##enableProxyAuthentication", ref this.UseProxyAuthentication);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Proxy login:");
            ImGui.TableNextColumn();
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputText("##proxyLogin", ref ProxyLogin, 1000);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiEx.TextV($"Proxy password:");
            ImGui.TableNextColumn();
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputText("##proxyPassword", ref ProxyPassword, 1000);

            ImGui.EndTable();
        }
    }
}