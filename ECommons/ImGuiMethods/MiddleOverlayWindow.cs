using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;


public class MiddleOverlayWindow : Window, IDisposable
{
    private int? TopOffset = null;
    private Vector4? bgCol = null;
    private Vector2 pos = Vector2.Zero;
    private WindowSystem ws = new();
    private Action draw;
    private bool disposed = false;
    public MiddleOverlayWindow(string name, Action draw, int? topOffset = null, Vector4? bgCol = null) : base(name, ImGuiWindowFlags.NoInputs
        | ImGuiWindowFlags.NoNav
        | ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoFocusOnAppearing
        | ImGuiWindowFlags.NoSavedSettings
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.AlwaysAutoResize, true)
    {
        TopOffset = topOffset;
        this.bgCol = bgCol;
        this.draw = draw;
        ws.AddWindow(this);
        IsOpen = true;
        Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
    }

    public override bool DrawConditions()
    {
        return Svc.Objects.LocalPlayer != null;
    }

    public override void Draw()
    {
        draw();
        pos = ImGui.GetWindowSize();
    }

    public override void PreDraw()
    {
        base.PreDraw();
        ImGui.SetNextWindowPos(new Vector2(ImGuiHelpers.MainViewport.Size.X / 2 - pos.X / 2,
            TopOffset ?? ImGuiHelpers.MainViewport.Size.Y / 3));
        if(bgCol.HasValue) ImGui.PushStyleColor(ImGuiCol.WindowBg, bgCol.Value);
    }

    public override void PostDraw()
    {
        base.PostDraw();
        if(bgCol.HasValue) ImGui.PopStyleColor();
    }

    public void Dispose()
    {
        if(!disposed)
        {
            disposed = true;
            Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
            ws.RemoveWindow(this);
        }
    }

}
