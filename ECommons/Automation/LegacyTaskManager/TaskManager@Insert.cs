using ECommons.Throttlers;
using System;

namespace ECommons.Automation.LegacyTaskManager;
#nullable disable
public partial class TaskManager
{
    /// <summary>
    /// Adds a task into the start of the main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void Insert(Func<bool?> task, string name = null)
    {
        Tasks.Insert(0, new(task, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Func<bool?> task, int timeLimitMs, string name = null)
    {
        Tasks.Insert(0, new(task, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Func<bool?> task, bool abortOnTimeout, string name = null)
    {
        Tasks.Insert(0, new(task, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Func<bool?> task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        Tasks.Insert(0, new(task, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }
    public void Insert(Action task, string name = null)
    {
        Tasks.Insert(0, new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Action task, int timeLimitMs, string name = null)
    {
        Tasks.Insert(0, new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Action task, bool abortOnTimeout, string name = null)
    {
        Tasks.Insert(0, new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    public void Insert(Action task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        Tasks.Insert(0, new(() => { task(); return true; }, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }

    public void InsertDelayNext(int delayMS, bool useFrameThrottler = false) => InsertDelayNext("ECommonsGenericDelay", delayMS, useFrameThrottler);
    public void InsertDelayNext(string uniqueName, int delayMS, bool useFrameThrottler = false)
    {
        if (useFrameThrottler)
        {
            Insert(() => FrameThrottler.Check(uniqueName), $"FrameThrottler.Check({uniqueName})");
            Insert(() => FrameThrottler.Throttle(uniqueName, delayMS), $"FrameThrottler.Throttle({uniqueName}, {delayMS})");
        }
        else
        {
            Insert(() => EzThrottler.Check(uniqueName), $"EzThrottler.Check({uniqueName})");
            Insert(() => EzThrottler.Throttle(uniqueName, delayMS), $"EzThrottler.Throttle({uniqueName}, {delayMS})");
        }
        MaxTasks += 2;
    }
}
