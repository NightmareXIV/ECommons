using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;

namespace ECommons.ImGuiMethods;
public abstract class NotifyWindow : Window
{
    public NotifyWindow(string name): base(name, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize, true)
    {
        this.RespectCloseHotkey = false;
        this.ShowCloseButton = false;
    }

    public override void Draw()
    {
        try
        {
            DrawContent();
        }
        catch (Exception e)
        {
            e.Log();
        }
        ImGuiViewportPtr mainViewport = ImGuiHelpers.MainViewport;
        this.Position = ImGuiHelpers.MainViewport.Size / 2f - ImGui.GetWindowSize() / 2f;
    }

    public abstract void DrawContent();
}
