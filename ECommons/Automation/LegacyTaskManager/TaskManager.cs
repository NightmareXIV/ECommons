using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.Automation.LegacyTaskManager;
#nullable disable

public partial class TaskManager : IDisposable
{
    private static readonly List<TaskManager> Instances = new();
    /// <summary>
    /// Number of tasks that were registered in current cycle. Increases each time a task is enqueued and resets once there are no more tasks.
    /// </summary>
    public int MaxTasks { get; private set; } = 0;
    /// <summary>
    /// After this time limit has passed, a task will be given up.
    /// </summary>
    public int TimeLimitMS = 10000;
    /// <summary>
    /// Whether to abort execution on timeout and clear all remaining tasks or not.
    /// </summary>
    public bool AbortOnTimeout = false;
    /// <summary>
    /// Tick count (<see cref="Environment.TickCount64"/>) at which current task will be aborted
    /// </summary>
    public long AbortAt { get; private set; } = 0;
    TaskManagerTask CurrentTask = null;
    public string CurrentTaskName => CurrentTask?.Name;
    public List<string> TaskStack => ImmediateTasks.Select(x => x.Name).Union(Tasks.Select(x => x.Name)).ToList();
    /// <summary>
    /// Amount of currently queued tasks, including one that is currently being executed
    /// </summary>
    public int NumQueuedTasks => Tasks.Count + ImmediateTasks.Count + (CurrentTask == null ? 0 : 1);
    /// <summary>
    /// Whether to redirect timeout errors into Verbose channel
    /// </summary>
    public bool TimeoutSilently = false;
    /// <summary>
    /// Whether to output debug information into PluginLog
    /// </summary>
    public bool ShowDebug = true;
    Action<string> LogTimeout => TimeoutSilently ? PluginLog.Verbose : PluginLog.Warning;

    List<TaskManagerTask> Tasks = new();
    List<TaskManagerTask> ImmediateTasks = new();

    /// <summary>
    /// Initializes new instance of <see cref="TaskManager"/>.
    /// </summary>
    public TaskManager()
    {
        Svc.Framework.Update += Tick;
        Instances.Add(this);
    }


    /// <summary>
    /// Sets step mode, when enabled task manager won't execute tasks automatically and will wait for Step command from you.
    /// </summary>
    /// <param name="enabled"></param>
    public void SetStepMode(bool enabled)
    {
        Svc.Framework.Update -= Tick;
        if (!enabled)
        {
            Svc.Framework.Update += Tick;
        }
    }

    /// <summary>
    /// Manually execute task manager cycle.
    /// </summary>
    public void Step() => Tick(null);

    /// <summary>
    /// Disposes task manager, stopping all tasks immediately.
    /// </summary>
    [Obsolete($"Task managers will be disposed automatically on {nameof(ECommonsMain.Dispose)} call. Use this if you need to dispose task manager before that.")]
    public void Dispose()
    {
        Svc.Framework.Update -= Tick;
        Instances.Remove(this);
    }

    internal static void DisposeAll()
    {
        int i = 0;
        foreach (var manager in Instances)
        {
            i++;
            Svc.Framework.Update -= manager.Tick;
        }
        if (i > 0)
        {
            PluginLog.Debug($"Auto-disposing {i} task managers");
        }
        Instances.Clear();
    }

    /// <summary>
    /// Whether TaskManager is currently executing a task.
    /// </summary>
    public bool IsBusy => CurrentTask != null || Tasks.Count > 0 || ImmediateTasks.Count > 0;

    public void Abort()
    {
        Tasks.Clear();
        ImmediateTasks.Clear();
        CurrentTask = null;
        MaxTasks = 0;
    }

    void Tick(object _)
    {
        if (CurrentTask == null)
        {
            if (ImmediateTasks.TryDequeue(out CurrentTask))
            {
                if (ShowDebug)
                    PluginLog.Debug($"Starting to execute immediate task: {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name}");
                AbortAt = Environment.TickCount64 + CurrentTask.TimeLimitMS;
            }
            else if (Tasks.TryDequeue(out CurrentTask))
            {
                if (ShowDebug)
                    PluginLog.Debug($"Starting to execute task: {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name}");
                AbortAt = Environment.TickCount64 + CurrentTask.TimeLimitMS;
            }
            else
            {
                MaxTasks = 0;
            }
        }
        else
        {
            try
            {
                var result = CurrentTask.Action();
                if (result == true)
                {
                    if (ShowDebug)
                        PluginLog.Debug($"Task {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name} completed successfully");
                    CurrentTask = null;
                }
                else if (result == false)
                {
                    if (Environment.TickCount64 > AbortAt)
                    {
                        if (CurrentTask.AbortOnTimeout)
                        {
                            LogTimeout($"Clearing {Tasks.Count} remaining tasks because of timeout");
                            Tasks.Clear();
                            ImmediateTasks.Clear();
                        }
                        throw new TimeoutException($"Task {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name} took too long to execute");
                    }
                }
                else
                {
                    PluginLog.Warning($"Clearing {Tasks.Count} remaining tasks because there was a signal from task {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name} to abort");
                    Abort();
                }
            }
            catch (TimeoutException e)
            {
                LogTimeout($"{e.Message}\n{e.StackTrace}");
                CurrentTask = null;
            }
            catch (Exception e)
            {
                e.Log();
                CurrentTask = null;
            }
        }
    }
}
