using ECommons.Throttlers;
using System;

namespace ECommons.Automation.LegacyTaskManager;
#nullable disable
public partial class TaskManager
{
    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Func<bool?> task, string name = null)
    {
        ImmediateTasks.Add(new(task, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Func<bool?> task, int timeLimitMs, string name = null)
    {
        ImmediateTasks.Add(new(task, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Func<bool?> task, bool abortOnTimeout, string name = null)
    {
        ImmediateTasks.Add(new(task, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Func<bool?> task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        ImmediateTasks.Add(new(task, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Action task, string name = null)
    {
        ImmediateTasks.Add(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Action task, int timeLimitMs, string name = null)
    {
        ImmediateTasks.Add(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Action task, bool abortOnTimeout, string name = null)
    {
        ImmediateTasks.Add(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void EnqueueImmediate(Action task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        ImmediateTasks.Add(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a set delay into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="delayMS"></param>
    /// <param name="useFrameThrottler"></param>
    public void DelayNextImmediate(int delayMS, bool useFrameThrottler = false) => DelayNextImmediate("ECommonsGenericDelay", delayMS, useFrameThrottler);

    /// <summary>
    /// Adds a set delay into the end of immediate queue. Whenever immediate queue is present, tasks from it will be executed before returning to the main queue.
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="delayMS"></param>
    /// <param name="useFrameThrottler"></param>
    public void DelayNextImmediate(string uniqueName, int delayMS, bool useFrameThrottler = false)
    {
        if (useFrameThrottler)
        {
            EnqueueImmediate(() => FrameThrottler.Throttle(uniqueName, delayMS), $"FrameThrottler.Throttle({uniqueName}, {delayMS})");
            EnqueueImmediate(() => FrameThrottler.Check(uniqueName), $"FrameThrottler.Check({uniqueName})");
        }
        else
        {
            EnqueueImmediate(() => EzThrottler.Throttle(uniqueName, delayMS), $"EzThrottler.Throttle({uniqueName}, {delayMS})");
            EnqueueImmediate(() => EzThrottler.Check(uniqueName), $"EzThrottler.Check({uniqueName})");
        }
        MaxTasks += 2;
    }
}
