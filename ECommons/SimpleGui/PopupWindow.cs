using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.SimpleGui;
/// <summary>
/// Allows creation of single-use popup windows. Disposes itself upon being closed.
/// </summary>
public class PopupWindow : Window
{
    private Action drawAction;
    private WindowSystem WindowSystem;
    public PopupWindow(Action content) : base($"{Svc.PluginInterface.Manifest.Name} - Notification##{Guid.NewGuid()}", ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoCollapse, true)
    {
        Configure(content);
    }

    public PopupWindow(string title, Action content) : base($"{title}##{Guid.NewGuid()}", ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoCollapse, true)
    {
        Configure(content);
    }

    [MemberNotNull(nameof(drawAction), nameof(WindowSystem))]
    private void Configure(Action content)
    {
        this.SetSizeConstraints(new(100), new(float.MaxValue));
        drawAction = content;
        RespectCloseHotkey = false;
        AllowPinning = false;
        WindowSystem = new();
        WindowSystem.AddWindow(this);
        IsOpen = true;
        Svc.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
    }

    public override void Draw()
    {
        drawAction?.Invoke();
    }

    public override void OnClose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
    }
}
