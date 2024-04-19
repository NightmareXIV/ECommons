using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;

namespace ECommons.Automation.NeoTaskManager;
/// <summary>
/// NeoTaskManager provides various benefits over previous task manager: increased speed, no need for immediate tasks, easier task creation and scheduling. Work in progress! Breaking changes may occur.
/// </summary>
public partial class TaskManager : IDisposable
{
    private static readonly List<TaskManager> Instances = [];
    internal static void DisposeAll()
    {
        foreach(var x in Instances.ToArray())
        {
            x.Dispose();
        }
    }

    /// <summary>
    /// Default configuration that will be used for tasks. Default configuration must not contain any null statements.
    /// </summary>
    public readonly TaskManagerConfiguration DefaultConfiguration;

    /// <summary>
    /// Amount of tasks that a TaskManager ever observed. Resets to 0 when there are no more tasks.
    /// </summary>
    public int MaxTasks { get; private set; } = 0;
    public int NumQueuedTasks => Tasks.Count + (CurrentTask == null ? 0 : 1);

    /// <summary>
    /// Indicates whether TaskManager is currently executing tasks
    /// </summary>
    public bool IsBusy => Tasks.Count != 0 || CurrentTask != null;

    /// <summary>
    /// List of currently enqueued tasks. You can modify list directly if you wish, but only ever do that from Framework.Update event.
    /// </summary>
    public List<TaskManagerTask> Tasks { get; init; } = [];

    /// <summary>
    /// A task that is currently being executed.
    /// </summary>
    public TaskManagerTask? CurrentTask { get; private set; } = null;

    /// <summary>
    /// Amount of milliseconds remaining before the currently executing task will be aborted
    /// </summary>
    public long RemainingTimeMS
    {
        get => AbortAt - Environment.TickCount64;
        set => AbortAt = Environment.TickCount64 + value;
    }

    /// <summary>
    /// Configures whether debug Step Mode is to be used. While in Step Mode, tasks are not advanced automatically and you must use Step method to advance tasks. Additionally, tasks will ignore time limit in Step Mode.
    /// </summary>
    public bool StepMode
    {
        get => stepMode;
        set
        {
            stepMode = value;
            if (stepMode == true) Svc.Framework.Update -= Tick;
            if (stepMode == false) Svc.Framework.Update += Tick;
        }
    }

    private long AbortAt = 0;
    private bool stepMode = false;

    public TaskManager(TaskManagerConfiguration defaultConfiguration)
    {
				DefaultConfiguration = new TaskManagerConfiguration()
				{
						AbortOnTimeout = true,
						AbortOnError = true,
						ShowDebug = false,
						ShowError = true,
						TimeLimitMS = 30000,
						TimeoutSilently = false,
				}.With(defaultConfiguration);
				DefaultConfiguration.AssertNotNull();
        Svc.Framework.Update += Tick;
        Instances.Add(this);
    }

    /// <summary>
    /// Any task managers will be disposed automatically on <see cref="ECommonsMain.Dispose"/> call, but you may dispose it earlier if you need to.
    /// </summary>
    public void Dispose()
    {
        Svc.Framework.Update -= Tick;
        Instances.Remove(this);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Immediately clears current task queue.
    /// </summary>
    public void Abort()
    {
        Tasks.Clear();
        AbortAt = 0;
        CurrentTask = null;
    }

    public void Step()
    {
        if (!StepMode) throw new InvalidOperationException("Can not use step function outside of step mode");
        this.Tick(null);
    }

    private void Tick(object? framework)
    {
        if (Tasks.Count > 0 || CurrentTask != null)
        {
            DefaultConfiguration.AssertNotNull();
            if (CurrentTask == null)
            {
                CurrentTask = Tasks[0];
                Tasks.RemoveAt(0);
                AbortAt = 0;
            }
            var TimeLimitMS = CurrentTask.Configuration?.TimeLimitMS ?? this.DefaultConfiguration.TimeLimitMS!.Value;
            var AbortOnTimeout = CurrentTask.Configuration?.AbortOnTimeout ?? this.DefaultConfiguration.AbortOnTimeout!.Value;
            var AbortOnError = CurrentTask.Configuration?.AbortOnError ?? this.DefaultConfiguration.AbortOnError!.Value;
            var TimeoutSilently = CurrentTask.Configuration?.TimeoutSilently ?? this.DefaultConfiguration.TimeoutSilently!.Value;
            var ShowDebug = CurrentTask.Configuration?.ShowDebug ?? this.DefaultConfiguration.ShowDebug!.Value;
            var ShowError = CurrentTask.Configuration?.ShowError ?? this.DefaultConfiguration.ShowError!.Value;
            if(Tasks.Count > MaxTasks) MaxTasks = Tasks.Count;
            try
            {
                if (AbortAt == 0)
                {
                    RemainingTimeMS = TimeLimitMS;
                    Log($"→Starting to execute task {CurrentTask.Name}, timeout={RemainingTimeMS}", ShowDebug);
                }
                if(RemainingTimeMS < 0)
                {
                    Log($"→→Task timed out {CurrentTask.Name}", ShowDebug);
                    throw new TaskTimeoutException();
                }
                var result = CurrentTask.Function();
                if(result == true)
                {
                    Log($"→→Task completed successfully {CurrentTask.Name}", ShowDebug);
                    CurrentTask = null;
                }
                else if(result == null)
                {
                    Log($"→→Received abort request from task {CurrentTask.Name}", ShowDebug);
                    Abort();
                }
            }
            catch(TaskTimeoutException e)
            {
                if (!TimeoutSilently)
                {
                    e.LogWarning();
                }
                if (AbortOnTimeout)
                {
                    Abort();
                }
                else
                {
                    CurrentTask = null;
                }
            }
            catch (Exception e)
            {
                if (ShowError)
                {
                    e.Log();
                }
                if (AbortOnError)
                {
                    Abort();
                }
                else
                {
                    CurrentTask = null;
                }
            }
            return;
        }
        if (MaxTasks != 0 && CurrentTask == null) MaxTasks = 0;
    }

    private void Log(string message, bool toConsole)
    {
        if (toConsole)
        {
            PluginLog.Debug("[NeoTaskManager] " + message);
        }
        else
        {
            InternalLog.Debug("[NeoTaskManager] " + message);
        }
    }
}
