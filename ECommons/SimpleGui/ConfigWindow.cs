using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Microsoft.Win32.SafeHandles;
using Reloaded.Hooks.Definitions.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.SimpleGui
{
    internal class ConfigWindow : Window
    {
        public ConfigWindow(string name) : base(name)
        {
            this.SizeConstraints = new()
            {
                MinimumSize = new(200, 200),
                MaximumSize = new(float.MaxValue, float.MaxValue)
            };
        }

        public override void Draw()
        {
            GenericHelpers.Safe(ConfigGui.Draw);
        }

        public override void OnOpen()
        {
            ConfigGui.OnOpen?.Invoke();
        }

        public override void OnClose()
        {
            if(ConfigGui.Config != null)
            {
                Svc.PluginInterface.SavePluginConfig(ConfigGui.Config);
                Notify.Success("Configuration saved");
            }
            ConfigGui.OnClose?.Invoke();
        }
    }
}
