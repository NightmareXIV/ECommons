using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;

namespace ECommons.ImGuiMethods;
public abstract class NotifyWindow : Window
{
    public NotifyWindow(string name) : base(name, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize, true)
    {
        RespectCloseHotkey = false;
        ShowCloseButton = false;
    }

    public override void Draw()
    {
        try
        {
            DrawContent();
        }
        catch(Exception e)
        {
            e.Log();
        }
        var mainViewport = ImGuiHelpers.MainViewport;
        Position = ImGuiHelpers.MainViewport.Size / 2f - ImGui.GetWindowSize() / 2f;
    }

    public abstract void DrawContent();
}
