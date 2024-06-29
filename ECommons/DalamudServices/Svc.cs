using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons.Logging;
using System;

namespace ECommons.DalamudServices;
#nullable disable

public class Svc
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] public static IBuddyList Buddies { get; private set; }
    [PluginService] public static IChatGui Chat { get; private set; }
    [PluginService] public static IClientState ClientState { get; private set; }
    [PluginService] public static ICommandManager Commands { get; private set; }
    [PluginService] public static ICondition Condition { get; private set; }
    [PluginService] public static IDataManager Data { get; private set; }
    [PluginService] public static IFateTable Fates { get; private set; }
    [PluginService] public static IFlyTextGui FlyText { get; private set; }
    [PluginService] public static IFramework Framework { get; private set; }
    [PluginService] public static IGameGui GameGui { get; private set; }
    [PluginService] public static IGameNetwork GameNetwork { get; private set; }
    [PluginService] public static IJobGauges Gauges { get; private set; }
    [PluginService] public static IKeyState KeyState { get; private set; }
    [PluginService] public static IObjectTable Objects { get; private set; }
    [PluginService] public static IPartyFinderGui PfGui { get; private set; }
    [PluginService] public static IPartyList Party { get; private set; }
    [PluginService] public static ISigScanner SigScanner { get; private set; }
    [PluginService] public static ITargetManager Targets { get; private set; }
    [PluginService] public static IToastGui Toasts { get; private set; }
    [PluginService] public static IGameConfig GameConfig { get; private set; }
    [PluginService] public static IGameLifecycle GameLifecycle { get; private set; }
    [PluginService] public static IGamepadState GamepadState { get; private set; }
    [PluginService] public static IDtrBar DtrBar { get; private set; }
    [PluginService] public static IDutyState DutyState { get; private set; }
    [PluginService] public static IGameInteropProvider Hook { get; private set; }
    [PluginService] public static ITextureProvider Texture { get; private set; }
    [PluginService] public static IPluginLog Log { get; private set; }
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; private set; }
    [PluginService] public static IAetheryteList AetheryteList { get; private set; }
    [PluginService] public static IAddonEventManager AddonEventManager { get; private set; }
    [PluginService] public static IGameInventory GameInventory { get; private set; }
    [PluginService] public static ITextureSubstitutionProvider TextureSubstitution { get; private set; }
    [PluginService] public static ITitleScreenMenu TitleScreenMenu { get; private set; }
    [PluginService] public static INotificationManager NotificationManager { get; private set; }
    [PluginService] public static IContextMenu ContextMenu { get; private set; }



		internal static bool IsInitialized = false;
    public static void Init(IDalamudPluginInterface pi)
    {
        if (IsInitialized)
        {
            PluginLog.Debug("Services already initialized, skipping");
        }
        IsInitialized = true;
        try
        {
            pi.Create<Svc>();
        }
        catch(Exception ex)
        {
            ex.Log();
        }
    }
}