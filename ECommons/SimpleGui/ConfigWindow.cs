﻿using Dalamud.Interface.Windowing;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using System.Runtime.ConstrainedExecution;

namespace ECommons.SimpleGui;

public class ConfigWindow : Window
{
    public ConfigWindow() : base($"{DalamudReflector.GetPluginName()} v{ECommonsMain.Instance.GetType().Assembly.GetName().Version}###{DalamudReflector.GetPluginName()}")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(200, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        GenericHelpers.Safe(EzConfigGui.Draw);
    }

    public override void OnOpen()
    {
        EzConfigGui.OnOpen?.Invoke();
    }

    public override void OnClose()
    {
        if(EzConfigGui.Config != null)
        {
            Svc.PluginInterface.SavePluginConfig(EzConfigGui.Config);
            Notify.Success("Configuration saved");
        }
        if(EzConfig.Config != null)
        {
            EzConfig.Save();
            Notify.Success("Configuration saved");
        }
        EzConfigGui.OnClose?.Invoke();
    }
}
