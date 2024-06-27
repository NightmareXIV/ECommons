using ECommons.Throttlers;
using System;

namespace ECommons.Automation.LegacyTaskManager;
#nullable disable
public partial class TaskManager
{
    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void Enqueue(Func<bool?> task, string name = null)
    {
        Tasks.Add(new(task, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="name"></param>
    public void Enqueue(Func<bool?> task, int timeLimitMs, string name = null)
    {
        Tasks.Add(new(task, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void Enqueue(Func<bool?> task, bool abortOnTimeout, string name = null)
    {
        Tasks.Add(new(task, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void Enqueue(Func<bool?> task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        Tasks.Add(new(task, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void Enqueue(Action task, string name = null)
    {
        Tasks.Add(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="name"></param>
    public void Enqueue(Action task, int timeLimitMs, string name = null)
    {
        Tasks.Add(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void Enqueue(Action task, bool abortOnTimeout, string name = null)
    {
        Tasks.Add(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a task into the end of main queue.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeLimitMs"></param>
    /// <param name="abortOnTimeout"></param>
    /// <param name="name"></param>
    public void Enqueue(Action task, int timeLimitMs, bool abortOnTimeout, string name = null)
    {
        Tasks.Add(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout, name));
        MaxTasks++;
    }

    /// <summary>
    /// Adds a set delay into the end of main queue.
    /// </summary>
    /// <param name="delayMS"></param>
    /// <param name="useFrameThrottler"></param>
    public void DelayNext(int delayMS, bool useFrameThrottler = false) => DelayNext("ECommonsGenericDelay", delayMS, useFrameThrottler);

    /// <summary>
    /// Adds a set delay into the end of main queue.
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="delayMS"></param>
    /// <param name="useFrameThrottler"></param>
    public void DelayNext(string uniqueName, int delayMS, bool useFrameThrottler = false)
    {
        if (useFrameThrottler)
        {
            Enqueue(() => FrameThrottler.Throttle(uniqueName, delayMS), $"FrameThrottler.Throttle({uniqueName}, {delayMS})");
            Enqueue(() => FrameThrottler.Check(uniqueName), $"FrameThrottler.Check({uniqueName})");
        }
        else
        {
            Enqueue(() => EzThrottler.Throttle(uniqueName, delayMS), $"EzThrottler.Throttle({uniqueName}, {delayMS})");
            Enqueue(() => EzThrottler.Check(uniqueName), $"EzThrottler.Check({uniqueName})");
        }
        MaxTasks += 2;
    }
}
