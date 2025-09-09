using Dalamud.Interface.Windowing;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.Reflection;
using System.Xml.Linq;

namespace ECommons.SimpleGui;
#nullable disable

public class ConfigWindow : Window
{
    public string? ConfigWindowName;
    public ConfigWindow(string? name = null) : base($"{name ?? DefaultPluginName}###{DalamudReflector.GetPluginName()}")
    {
        ConfigWindowName = name;
        SizeConstraints = new()
        {
            MinimumSize = new(200, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public static string DefaultPluginName => $"{DalamudReflector.GetPluginName()} v{ECommonsMain.Instance.GetType().Assembly.GetName().Version}";

    public void SetSuffix(string suffix)
    {
        this.WindowName = $"{ConfigWindowName ?? DefaultPluginName}{(suffix == null?"":$" {suffix}")}###{DalamudReflector.GetPluginName()}";
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
