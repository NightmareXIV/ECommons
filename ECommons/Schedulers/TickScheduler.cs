using ECommons.DalamudServices;
using ECommons.Logging;
using System;

namespace ECommons.Schedulers;

public class TickScheduler : IScheduler
{
    private long executeAt;
    private Action function;
    private bool disposed = false;

    public TickScheduler(Action function, long delayMS = 0)
    {
        executeAt = Environment.TickCount64 + delayMS;
        this.function = function;
        Svc.Framework.Update += Execute;
    }

    public void Dispose()
    {
        if(!disposed)
        {
            Svc.Framework.Update -= Execute;
        }
        disposed = true;
    }

    private void Execute(object _)
    {
        if(Environment.TickCount64 < executeAt) return;
        try
        {
            function();
        }
        catch(Exception e)
        {
            PluginLog.Error(e.Message + "\n" + e.StackTrace ?? "");
        }
        Dispose();
    }
}