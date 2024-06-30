using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;
using System;

namespace ECommons.DalamudServices.Legacy;

public static class LegacyHelpers
{
    public static void PrintChat(this IChatGui chatGui, XivChatEntry entry) => Svc.Chat.Print(entry);

    public static void SetTarget(this ITargetManager targetManager, IGameObject obj)
    {
        targetManager.Target = obj;
    }

    public static void AddNotification(this IUiBuilder builder, string message, string? pluginName = null, NotificationType type = NotificationType.Info, int timeout = 3000)
    {
        Svc.NotificationManager.AddNotification(new()
        {
            Content = message,
            Title = pluginName ?? Svc.PluginInterface.InternalName,
            InitialDuration = TimeSpan.FromMilliseconds(timeout),
            Type = type,
        });
    }
}
