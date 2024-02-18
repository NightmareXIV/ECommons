using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;

namespace ECommons.DalamudServices.Legacy;

public static class LegacyHelpers
{
    public static void PrintChat(this IChatGui chatGui, XivChatEntry entry) => Svc.Chat.Print(entry);

    public static void SetTarget(this ITargetManager targetManager, GameObject obj)
    {
        targetManager.Target = obj;
    }
}
