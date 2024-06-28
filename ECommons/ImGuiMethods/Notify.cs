using Dalamud.Interface.ImGuiNotification;
using ECommons.DalamudServices;
using ECommons.Reflection;
using ECommons.Schedulers;

namespace ECommons.ImGuiMethods;

public static class Notify
{
    public static void Success(string s)
    {
        _ = new TickScheduler(delegate
        {
            Svc.NotificationManager.AddNotification(new Notification() { Content = s, Title = DalamudReflector.GetPluginName(), Type = NotificationType.Success });
        });
    }

    public static void Info(string s)
    {
        _ = new TickScheduler(delegate
        {
            Svc.NotificationManager.AddNotification(new Notification() { Content = s, Title = DalamudReflector.GetPluginName(), Type = NotificationType.Info });
        });
    }

    public static void Error(string s)
    {
        _ = new TickScheduler(delegate
        {
            Svc.NotificationManager.AddNotification(new Notification() { Content = s, Title = DalamudReflector.GetPluginName(), Type = NotificationType.Error });
        });
    }

    public static void Warning(string s)
    {
        _ = new TickScheduler(delegate
        {
            Svc.NotificationManager.AddNotification(new Notification() { Content = s, Title = DalamudReflector.GetPluginName(), Type = NotificationType.Warning });
        });
    }

    public static void Plain(string s)
    {
        _ = new TickScheduler(delegate
        {
            Svc.NotificationManager.AddNotification(new Notification() { Content = s, Title = DalamudReflector.GetPluginName(), Type = NotificationType.None });
        });
    }
}
