using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation
{
    public class TaskManager : IDisposable
    {
        public int TimeLimitMS = 10000;
        public long AbortAt { get; private set; } = 0;
        Func<bool> CurrentTask = null;

        Queue<Func<bool>> Tasks = new();

        public TaskManager()
        {
            Svc.Framework.Update += Tick;
        }

        public void Dispose()
        {
            Svc.Framework.Update -= Tick;
        }

        public bool IsBusy => CurrentTask != null || Tasks.Count > 0;

        public void Enqueue(Func<bool> task)
        {
            Tasks.Enqueue(task);
        }

        void Tick(object _)
        {
            if (CurrentTask == null)
            {
                if (Tasks.TryDequeue(out CurrentTask))
                {
                    PluginLog.Debug($"Starting to execute new task");
                    AbortAt = Environment.TickCount64 + TimeLimitMS;
                }
            }
            else
            {
                try
                {
                    if (CurrentTask())
                    {
                        CurrentTask = null;
                        PluginLog.Debug($"Task completed successfully");
                    }
                    else
                    {
                        if (Environment.TickCount64 > AbortAt)
                        {
                            throw new TimeoutException("Task took too long to execute");
                        }
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
