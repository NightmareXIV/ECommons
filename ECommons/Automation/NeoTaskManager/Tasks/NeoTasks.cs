using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.NeoTaskManager.Tasks;

/// <summary>
/// A collection of functions that are very commonly used in various plugins
/// </summary>
public static unsafe class NeoTasks
{
    public static TaskManagerTask ApproachObjectViaAutomove(Func<IGameObject> getObjectFunc, float distance = 4f, TaskManagerConfiguration? configuration = null)
    {
        return new(() =>
        {
            var obj = getObjectFunc();
            if(obj == null) return false;
            if(!Player.Interactable) return false;
            if(AgentMap.Instance()->IsPlayerMoving)
            {
                if(Player.DistanceTo(obj) < distance && EzThrottler.Throttle("TaskFunction.AutomoveOff", 200))
                {
                    Chat.ExecuteCommand("/automove off");
                    return false;
                }
            }
            else
            {
                if(Player.DistanceTo(obj) < distance)
                {
                    return true;
                }
                if(obj.IsTarget())
                {
                    if(EzThrottler.Throttle("TaskFunction.Lockon"))
                    {
                        Chat.ExecuteCommand("/lockon on");
                        Chat.ExecuteCommand("/automove on");
                        return false;
                    }
                }
                else
                {
                    if(obj.IsTargetable && EzThrottler.Throttle("TaskFunction.SetTarget", 200))
                    {
                        Svc.Targets.Target = obj;
                        return false;
                    }
                }
            }
            return false;
        }, $"ApproachObjectViaAutomove({getObjectFunc()}, {distance})", configuration);
    }

    public static TaskManagerTask InteractWithObject(Func<IGameObject> getObjectFunc, bool checkLos = false, TaskManagerConfiguration? configuration = null)
    {
        return new(() =>
        {
            var obj = getObjectFunc();
            if(obj == null) return false;
            if(obj.IsTargetable && !Player.IsAnimationLocked && Player.Interactable)
            {
                if(obj.IsTarget())
                {
                    if(EzThrottler.Throttle("TaskFunction.Interact"))
                    {
                        TargetSystem.Instance()->InteractWithObject(obj.Struct(), checkLos);
                        return true;
                    }
                }
                else
                {
                    if(EzThrottler.Throttle("TaskFunction.SetTarget", 200))
                    {
                        Svc.Targets.Target = obj;
                        return false;
                    }
                }
            }
            return false;
        }, $"InteractWithObject({getObjectFunc()}, {checkLos})", configuration);
    }

    public static TaskManagerTask WaitForScreenAndPlayer(TaskManagerConfiguration? configuration = null)
    {
        return new(() => Player.Interactable && GenericHelpers.IsScreenReady(), "Wait for screen fadeout complete", configuration);
    }

    public static TaskManagerTask WaitForNotOccupied(TaskManagerConfiguration? configuration = null)
    {
        return new(() => !GenericHelpers.IsOccupied() && Player.Interactable && GenericHelpers.IsScreenReady(), "Wait for screen fadeout complete", configuration);
    }
}
