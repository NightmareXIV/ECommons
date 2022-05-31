using Dalamud.Interface.Internal.Notifications;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public static class Notify
    {
        public static void Success(string s)
        {
            Svc.PluginInterface.UiBuilder.AddNotification(s, DalamudReflector.GetPluginName(), NotificationType.Success);
        }

        public static void Info(string s)
        {
            Svc.PluginInterface.UiBuilder.AddNotification(s, DalamudReflector.GetPluginName(), NotificationType.Info);
        }

        public static void Error(string s)
        {
            Svc.PluginInterface.UiBuilder.AddNotification(s, DalamudReflector.GetPluginName(), NotificationType.Error);
        }

        public static void Warning(string s)
        {
            Svc.PluginInterface.UiBuilder.AddNotification(s, DalamudReflector.GetPluginName(), NotificationType.Warning);
        }

        public static void Plain(string s)
        {
            Svc.PluginInterface.UiBuilder.AddNotification(s, DalamudReflector.GetPluginName(), NotificationType.None);
        }

        static void SafeNotification(string s, NotificationType type)
        {
            
        }
    }
}
