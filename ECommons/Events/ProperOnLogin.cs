using ECommons.Logging;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using ECommons.GameHelpers;

namespace ECommons.Events;
#nullable disable

public static class ProperOnLogin
{
    static HashSet<Action> RegisteredActions = new();
    static HashSet<Action> RegisteredActionsInteractable = new();
    static bool EventRegisteredAvailable = false;
    static bool EventRegisteredInteractable = false;

    public static bool PlayerPresent => Svc.ClientState.LocalPlayer != null && Svc.ClientState.LocalContentId != 0;

    public static void FireArtificially()
    {
        OnUpdateAvailable(null);
        OnUpdateInteractable(null);
    }

    [Obsolete("Use either RegisterAvailable or RegisterInteractable")]
    public static void Register(Action action, bool fireImmediately = false) => RegisterAvailable(action, fireImmediately);

    public static void RegisterAvailable(Action action, bool fireImmediately = false)
    {
        if (RegisteredActionsInteractable.Contains(action))
        {
            PluginLog.Warning($"{action.GetType().FullName} ProperOnLogin available event is already registered in interactable section!");
        }
        else if (RegisteredActions.Contains(action))
        {
            PluginLog.Warning($"{action.GetType().FullName} ProperOnLogin available event is already registered in available section!");
        }
        else
        {
            RegisteredActions.Add(action);
            PluginLog.Debug($"Registered ProperOnLogin available event: {action.GetType().FullName}");
            if (!EventRegisteredAvailable)
            {
                EventRegisteredAvailable = true;
                Svc.ClientState.Login += OnLoginAvailable;
                PluginLog.Debug("ProperOnLogin master available event registered");
            }
            if(fireImmediately && PlayerPresent)
            {
                GenericHelpers.Safe(action);
            }
        }
    }

    public static void RegisterInteractable(Action action, bool fireImmediately = false)
    {
        if (RegisteredActionsInteractable.Contains(action))
        {
            PluginLog.Warning($"{action.GetType().FullName} ProperOnLogin interactable event is already registered in interactable section!");
        }
        else if(RegisteredActions.Contains(action))
        {
            PluginLog.Warning($"{action.GetType().FullName} ProperOnLogin interactable event is already registered in available section!");
        }
        else
        {
            RegisteredActionsInteractable.Add(action);
            PluginLog.Debug($"Registered interactable ProperOnLogin event: {action.GetType().FullName}");
            if (!EventRegisteredInteractable)
            {
                EventRegisteredInteractable = true;
                Svc.ClientState.Login += OnLoginInteractable;
                PluginLog.Debug("ProperOnLogin interactable master event registered");
            }
            if (fireImmediately && Player.Interactable)
            {
                GenericHelpers.Safe(action);
            }
        }
    }

    [Obsolete("This event is automatically disposed together with ECommons")]
    public static void Unregister(Action action)
    {
        if (!RegisteredActions.Remove(action) && !RegisteredActionsInteractable.Remove(action))
        {
            PluginLog.Warning($"{action.GetType().FullName} ProperOnLogin event is not registered!");
        }
        else
        {
            PluginLog.Debug($"Unregistered ProperOnLogin event: {action.GetType().FullName}");
        }
    }

    static void OnLoginAvailable()
    {
        Svc.Framework.Update += OnUpdateAvailable;
        PluginLog.Debug("Registering ProperOnLogin Available event's framework update");
    }

    static void OnUpdateAvailable(object _)
    {
        if(PlayerPresent)
        {
            PluginLog.Debug("Firing ProperOnLogin Available event and unregistering framework update");
            Svc.Framework.Update -= OnUpdateAvailable;
            foreach(var x in RegisteredActions)
            {
                try
                {
                    x();
                }
                catch(Exception e)
                {
                    PluginLog.Error($"Exception while processing ProperOnLogin Available event in {x.GetType().FullName}");
                    e.Log();
                }
            }
        }
    }

    static void OnLoginInteractable()
    {
        Svc.Framework.Update += OnUpdateInteractable;
        PluginLog.Debug("Registering ProperOnLogin Interactable event's framework update");
    }

    static void OnUpdateInteractable(object _)
    {
        if (Player.Interactable)
        {
            PluginLog.Debug("Firing ProperOnLogin Interactable event and unregistering framework update");
            Svc.Framework.Update -= OnUpdateInteractable;
            foreach (var x in RegisteredActionsInteractable)
            {
                try
                {
                    x();
                }
                catch (Exception e)
                {
                    PluginLog.Error($"Exception while processing ProperOnLogin Interactable event in {x.GetType().FullName}");
                    e.Log();
                }
            }
        }
    }

    internal static void Dispose()
    {
        if (EventRegisteredAvailable)
        {
            Svc.ClientState.Login -= OnLoginAvailable;
            PluginLog.Debug("ProperOnLogin master available event unregistered");
        }
        if (EventRegisteredInteractable)
        {
            Svc.ClientState.Login -= OnLoginInteractable;
            PluginLog.Debug("ProperOnLogin master interactable event unregistered");
        }
    }
}
