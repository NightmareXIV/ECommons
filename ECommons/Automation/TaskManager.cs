using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.Throttlers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ECommons.Automation
{
    public class TaskManager : IDisposable
    {
        private static readonly List<TaskManager> Instances = new();
        public int TimeLimitMS = 10000;
        public bool AbortOnTimeout = false;
        public long AbortAt { get; private set; } = 0;
        TaskManagerTask CurrentTask = null;
        public int NumQueuedTasks => Tasks.Count + ImmediateTasks.Count + (CurrentTask == null ? 0 : 1);
        public bool TimeoutSilently = false;
        public bool ShowDebug = false;
        Action<string> LogTimeout => TimeoutSilently ? PluginLog.Verbose : PluginLog.Warning;

        Queue<TaskManagerTask> Tasks = new();
        Queue<TaskManagerTask> ImmediateTasks = new();

        public TaskManager()
        {
            Svc.Framework.Update += Tick;
            Instances.Add(this);
        }

        [Obsolete($"Task managers will be disposed automatically on {nameof(ECommonsMain.Dispose)} call. Use this if you need to dispose task manager before that.")]
        public void Dispose()
        {
            Svc.Framework.Update -= Tick;
            Instances.Remove(this);
        }

        internal static void DisposeAll()
        {
            int i = 0;
            foreach(var manager in Instances)
            {
                i++;
                Svc.Framework.Update -= manager.Tick;
            }
            if(i>0)
            {
                PluginLog.Debug($"Auto-disposing {i} task managers");
            }
            Instances.Clear();
        }

        public bool IsBusy => CurrentTask != null || Tasks.Count > 0 || ImmediateTasks.Count > 0;

        public void Enqueue(Func<bool?> task, string name = null)
        {
            Tasks.Enqueue(new(task, TimeLimitMS, AbortOnTimeout, name));
        }

        public void Enqueue(Func<bool?> task, int timeLimitMs, string name = null)
        {
            Tasks.Enqueue(new(task, timeLimitMs, AbortOnTimeout, name));
        }

        public void Enqueue(Func<bool?> task, bool abortOnTimeout, string name = null)
        {
            Tasks.Enqueue(new(task, TimeLimitMS, abortOnTimeout, name));
        }

        public void Enqueue(Func<bool?> task, int timeLimitMs, bool abortOnTimeout, string name = null)
        {
            Tasks.Enqueue(new(task, timeLimitMs, abortOnTimeout, name));
        }
        public void Enqueue(Action task, string name = null)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout, name));
        }

        public void Enqueue(Action task, int timeLimitMs, string name = null)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout, name));
        }

        public void Enqueue(Action task, bool abortOnTimeout, string name = null)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout, name));
        }

        public void Enqueue(Action task, int timeLimitMs, bool abortOnTimeout, string name = null)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout, name));
        }

        public void DelayNext(int delayMS, bool useFrameThrottler = false) => DelayNext("ECommonsGenericDelay", delayMS, useFrameThrottler);
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
        }

        public void DelayNextImmediate(int delayMS, bool useFrameThrottler = false) => DelayNext("ECommonsGenericDelay", delayMS, useFrameThrottler);
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
        }

        public void Abort()
        {
            Tasks.Clear();
            ImmediateTasks.Clear();
            CurrentTask = null;
        }

        public void EnqueueImmediate(Func<bool?> task, string name = null)
        {
            ImmediateTasks.Enqueue(new(task, TimeLimitMS, AbortOnTimeout, name));
        }

        public void EnqueueImmediate(Func<bool?> task, int timeLimitMs, string name = null)
        {
            ImmediateTasks.Enqueue(new(task, timeLimitMs, AbortOnTimeout, name));
        }

        public void EnqueueImmediate(Func<bool?> task, bool abortOnTimeout, string name = null)
        {
            ImmediateTasks.Enqueue(new(task, TimeLimitMS, abortOnTimeout, name));
        }

        public void EnqueueImmediate(Func<bool?> task, int timeLimitMs, bool abortOnTimeout, string name = null)
        {
            ImmediateTasks.Enqueue(new(task, timeLimitMs, abortOnTimeout, name));
        }

        public void EnqueueImmediate(Action task, string name = null)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout, name));
        }

        public void EnqueueImmediate(Action task, int timeLimitMs, string name = null)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout, name));
        }

        public void EnqueueImmediate(Action task, bool abortOnTimeout, string name = null)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout, name));
        }

        public void EnqueueImmediate(Action task, int timeLimitMs, bool abortOnTimeout, string name = null)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout, name));
        }

        void Tick(object _)
        {
            if (CurrentTask == null)
            {
                if (ImmediateTasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute immediate task: {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name}");
                    AbortAt = Environment.TickCount64 + CurrentTask.TimeLimitMS;
                }
                else if (Tasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute task: {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name}");
                    AbortAt = Environment.TickCount64 + CurrentTask.TimeLimitMS;
                }
            }
            else
            {
                try
                {
                    var result = CurrentTask.Action();
                    if (result == true)
                    {
                        PluginLog.Debug($"Task {CurrentTask.Name ?? CurrentTask.Action.GetMethodInfo()?.Name} completed successfully");
                        CurrentTask = null; 
                    }
                    else if(result == false)
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
}
