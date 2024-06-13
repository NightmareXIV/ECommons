using Dalamud.Interface.Windowing;
using ECommons.Logging;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.Reflection;

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
            PluginLog.Debug("Configuration saved");
        }
        if(EzConfig.Config != null)
        {
            EzConfig.Save();
						PluginLog.Debug("Configuration saved");
        }
        EzConfigGui.OnClose?.Invoke();
    }
}
