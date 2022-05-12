using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public class TimedMiddleOverlayWindow : Window, IDisposable
    {
        int? TopOffset = null;
        Vector4? bgCol = null;
        Vector2 pos = Vector2.Zero;
        WindowSystem ws = new();
        Action draw;
        long destroyAt;
        bool disposed = false;
        public TimedMiddleOverlayWindow(string name, long destroyAfterMS, Action draw, int? topOffset = null, Vector4? bgCol = null) : base(name, ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoNav
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoFocusOnAppearing
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.AlwaysAutoResize, true)
        {
            TopOffset = topOffset;
            this.bgCol = bgCol;
            this.destroyAt = Environment.TickCount64 + destroyAfterMS;
            this.draw = draw;
            ws.AddWindow(this);
            this.IsOpen = true;
            Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
        }

        public override bool DrawConditions()
        {
            return Svc.ClientState.LocalPlayer != null;
        }

        public override void Draw()
        {
            this.draw();
            pos = ImGui.GetWindowSize();
        }

        public override void PreDraw()
        {
            base.PreDraw();
            ImGui.SetNextWindowPos(new Vector2(ImGuiHelpers.MainViewport.Size.X / 2 - pos.X / 2,
                TopOffset ?? ImGuiHelpers.MainViewport.Size.Y / 3));
            if (bgCol.HasValue) ImGui.PushStyleColor(ImGuiCol.WindowBg, bgCol.Value);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (bgCol.HasValue) ImGui.PopStyleColor();
        }

        public override void Update()
        {
            if(Environment.TickCount64 > destroyAt)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
                ws.RemoveWindow(this);
            }
        }
    }
}
