using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using System;

namespace ECommons.ImGuiMethods;

public class PopupWindow : IDisposable
{
    public string Text = "";
    public PopupWindow(string Text) 
    { 
        this.Text = Text;
        Svc.PluginInterface.UiBuilder.Draw += UiBuilder_Draw;
    }

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= UiBuilder_Draw;
    }

    private void UiBuilder_Draw()
    {
        ImGuiHelpers.ForceNextWindowMainViewport();
    }
}
