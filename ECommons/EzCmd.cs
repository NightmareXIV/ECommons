using Dalamud.Game.Command;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System.Collections.Generic;
using static Dalamud.Game.Command.CommandInfo;

namespace ECommons;
#nullable disable

public static class EzCmd
{
    internal static List<string> RegisteredCommands = [];

    //[Obsolete("Please use Cmd Attribute to the method in IDalamudPlugin to Add your command.")]
    public static void Add(string command, IReadOnlyCommandInfo.HandlerDelegate action, string helpMessage = null, int displayOrder = -1)
    {
        RegisteredCommands.Add(command);
        var cInfo = new CommandInfo(action)
        {
            HelpMessage = helpMessage ?? "",
            ShowInHelp = helpMessage != null,
            DisplayOrder = displayOrder
        };
        /*GenericHelpers.Safe(delegate
        {
            cInfo.SetFoP("LoaderAssemblyName", Svc.PluginInterface.InternalName);
        });*/
        Svc.Commands.AddHandler(command, cInfo);
    }
}
