using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool TimeoutSilently = false;
        Action<string> LogTimeout => TimeoutSilently ? PluginLog.Verbose : PluginLog.Warning;
        string StackTrace => new StackTrace().GetFrames().Select(x => x.GetMethod()?.Name ?? "<unknown>").Join(" <- ");

        Queue<TaskManagerTask> Tasks = new();

        public TaskManager()
        {
            Svc.Framework.Update += Tick;
        }

        public void Dispose()
        {
            Svc.Framework.Update -= Tick;
        }

        public bool IsBusy => CurrentTask != null || Tasks.Count > 0;

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

        void Tick(object _)
        {
            if (CurrentTask == null)
            {
                if (Tasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute task: {StackTrace}");
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
                        PluginLog.Debug($"Task {StackTrace} completed successfully");
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
                            }
                            throw new TimeoutException($"Task {StackTrace} took too long to execute");
                        }
                    }
                    else
                    {
                        PluginLog.Debug($"Clearing {Tasks.Count} remaining tasks because there was a signal from task {StackTrace} to abort");
                        Tasks.Clear();
                        CurrentTask = null;
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
