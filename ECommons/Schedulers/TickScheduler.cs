using ECommons.DalamudServices;
using ECommons.Logging;
using System;

namespace ECommons.Schedulers;

public class TickScheduler : IScheduler
{
    private long ExecuteAt;
    private Action Action;
    public bool Disposed { get; private set; } = false;

    public TickScheduler(Action function, long delayMS = 0)
    {
        ExecuteAt = Environment.TickCount64 + delayMS;
        Action = function;
        Svc.Framework.Update += Execute;
    }

    public void Dispose()
    {
        if(!Disposed)
        {
            Svc.Framework.Update -= Execute;
        }
        Disposed = true;
    }

    private void Execute(object _)
    {
        if(Environment.TickCount64 < ExecuteAt) return;
        try
        {
            Action();
        }
        catch(Exception e)
        {
            PluginLog.Error(e.Message + "\n" + e.StackTrace ?? "");
        }
        Dispose();
    }
}