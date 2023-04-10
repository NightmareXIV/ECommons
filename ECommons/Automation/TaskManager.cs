using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation
{
    public class TaskManager : IDisposable
    {
        public int TimeLimitMS = 10000;
        public bool AbortOnTimeout = false;
        public long AbortAt { get; private set; } = 0;
        TaskManagerTask CurrentTask = null;
        public int NumQueuedTasks => Tasks.Count + ImmediateTasks.Count + (CurrentTask == null ? 0 : 1);

        Queue<TaskManagerTask> Tasks = new();
        Queue<TaskManagerTask> ImmediateTasks = new();

        public TaskManager()
        {
            Svc.Framework.Update += Tick;
        }

        public void Dispose()
        {
            Svc.Framework.Update -= Tick;
        }

        public bool IsBusy => CurrentTask != null || Tasks.Count > 0 || ImmediateTasks.Count > 0;

        public void Enqueue(Func<bool?> task)
        {
            Tasks.Enqueue(new(task, TimeLimitMS, AbortOnTimeout));
        }

        public void Enqueue(Func<bool?> task, int timeLimitMs)
        {
            Tasks.Enqueue(new(task, timeLimitMs, AbortOnTimeout));
        }

        public void Enqueue(Func<bool?> task, bool abortOnTimeout)
        {
            Tasks.Enqueue(new(task, TimeLimitMS, abortOnTimeout));
        }

        public void Enqueue(Func<bool?> task, int timeLimitMs, bool abortOnTimeout)
        {
            Tasks.Enqueue(new(task, timeLimitMs, abortOnTimeout));
        }
        public void Enqueue(Action task)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout));
        }

        public void Enqueue(Action task, int timeLimitMs)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout));
        }

        public void Enqueue(Action task, bool abortOnTimeout)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout));
        }

        public void Enqueue(Action task, int timeLimitMs, bool abortOnTimeout)
        {
            Tasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout));
        }

        public void Abort()
        {
            Tasks.Clear();
            ImmediateTasks.Clear();
            CurrentTask = null;
        }

        public void EnqueueImmediate(Func<bool?> task)
        {
            ImmediateTasks.Enqueue(new(task, TimeLimitMS, AbortOnTimeout));
        }

        public void EnqueueImmediate(Func<bool?> task, int timeLimitMs)
        {
            ImmediateTasks.Enqueue(new(task, timeLimitMs, AbortOnTimeout));
        }

        public void EnqueueImmediate(Func<bool?> task, bool abortOnTimeout)
        {
            ImmediateTasks.Enqueue(new(task, TimeLimitMS, abortOnTimeout));
        }

        public void EnqueueImmediate(Func<bool?> task, int timeLimitMs, bool abortOnTimeout)
        {
            ImmediateTasks.Enqueue(new(task, timeLimitMs, abortOnTimeout));
        }

        public void EnqueueImmediate(Action task)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, AbortOnTimeout));
        }

        public void EnqueueImmediate(Action task, int timeLimitMs)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, AbortOnTimeout));
        }

        public void EnqueueImmediate(Action task, bool abortOnTimeout)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, TimeLimitMS, abortOnTimeout));
        }

        public void EnqueueImmediate(Action task, int timeLimitMs, bool abortOnTimeout)
        {
            ImmediateTasks.Enqueue(new(() => { task(); return true; }, timeLimitMs, abortOnTimeout));
        }

        void Tick(object _)
        {
            if (CurrentTask == null)
            {
                if (ImmediateTasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute immediate task: {CurrentTask.Action.GetMethodInfo()?.Name}");
                    AbortAt = Environment.TickCount64 + CurrentTask.TimeLimitMS;
                }
                else if (Tasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute task: {CurrentTask.Action.GetMethodInfo()?.Name}");
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
                        PluginLog.Debug($"Task {CurrentTask.Action.GetMethodInfo()?.Name} completed successfully");
                        CurrentTask = null; 
                    }
                    else if(result == false)
                    {
                        if (Environment.TickCount64 > AbortAt)
                        {
                            if (CurrentTask.AbortOnTimeout)
                            {
                                PluginLog.Warning($"Clearing {Tasks.Count} remaining tasks because of timeout");
                                Tasks.Clear();
                                ImmediateTasks.Clear();
                            }
                            throw new TimeoutException($"Task {CurrentTask.Action.GetMethodInfo()?.Name} took too long to execute");
                        }
                    }
                    else
                    {
                        PluginLog.Warning($"Clearing {Tasks.Count} remaining tasks because there was a signal from task {CurrentTask.Action.GetMethodInfo()?.Name} to abort");
                        Abort();
                    }
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
