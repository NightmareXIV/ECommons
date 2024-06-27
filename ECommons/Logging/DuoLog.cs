using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using ECommons.Reflection;
using ECommons.Schedulers;

namespace ECommons.Logging;

public static class DuoLog
{
    public static void Information(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Information(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 3).Build(),
                Type = Svc.PluginInterface.GeneralChatType
            });
        });
    }

    public static void Debug(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Debug(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 4).Build(),
                Type = Svc.PluginInterface.GeneralChatType
            });
        });
    }

    public static void Verbose(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Verbose(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 5).Build(),
                Type = Svc.PluginInterface.GeneralChatType
            });
        });
    }

    public static void Warning(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Warning(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 540).Build(),
                Type = Svc.PluginInterface.GeneralChatType,
            });
        });
    }

    public static void Error(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Error(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 17).Build(),
                Type = Svc.PluginInterface.GeneralChatType,
            });
        });
    }

    public static void Fatal(string s)
    {
        var str = $"[{DalamudReflector.GetPluginName()}] {s}";
        PluginLog.Fatal(str);
        _ = new TickScheduler(delegate
        {
            Svc.Chat.Print(new()
            {
                Message = new SeStringBuilder().AddUiForeground(str, 19).Build(),
                Type = Svc.PluginInterface.GeneralChatType,
            });
        });
    }
}
