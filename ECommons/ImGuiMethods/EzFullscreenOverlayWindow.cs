using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace ECommons.ImGuiMethods;
public abstract class EzFullscreenOverlayWindow : Window
{
    public EzFullscreenOverlayWindow(string name) : base(name, ImGuiEx.OverlayFlags, true)
    {
        this.RespectCloseHotkey = false;
        this.IsOpen = true;
    }

    public virtual void PreDrawAction() { }

    public sealed override void PreDraw()
    {
        PreDrawAction();
        ImGui.SetNextWindowSize(ImGuiHelpers.MainViewport.Size);
        ImGuiHelpers.SetNextWindowPosRelativeMainViewport(Vector2.Zero);
    }
}
