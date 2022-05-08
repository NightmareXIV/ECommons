using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Gui.PartyFinder;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Libc;
using Dalamud.Game.Network;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace ECommons.DalamudServices
{
    public class Svc
    {
        [PluginService] static public DalamudPluginInterface PluginInterface { get; private set; }
        [PluginService] static public BuddyList Buddies { get; private set; }
        [PluginService] static public ChatGui Chat { get; private set; }
        [PluginService] static public ChatHandlers ChatHandlers { get; private set; }
        [PluginService] static public ClientState ClientState { get; private set; }
        [PluginService] static public CommandManager Commands { get; private set; }
        [PluginService] static public Condition Condition { get; private set; }
        [PluginService] static public DataManager Data { get; private set; }
        [PluginService] static public FateTable Fates { get; private set; }
        [PluginService] static public FlyTextGui FlyText { get; private set; }
        [PluginService] static public Framework Framework { get; private set; }
        [PluginService] static public GameGui GameGui { get; private set; }
        [PluginService] static public GameNetwork GameNetwork { get; private set; }
        [PluginService] static public JobGauges Gauges { get; private set; }
        [PluginService] static public KeyState KeyState { get; private set; }
        [PluginService] static public LibcFunction LibcFunction { get; private set; }
        [PluginService] static public ObjectTable Objects { get; private set; }
        [PluginService] static public PartyFinderGui PfGui { get; private set; }
        [PluginService] static public PartyList Party { get; private set; }
        [PluginService] static public SigScanner SigScanner { get; private set; }
        [PluginService] static public TargetManager Targets { get; private set; }
        [PluginService] static public ToastGui Toasts { get; private set; }
    }
}