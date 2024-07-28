using ECommons.DalamudServices;
using ECommons.Logging;
using System;

namespace ECommons.Schedulers;

public class ExecuteForScheduler : IScheduler
{
    private long stopExecAt;
    private Action function;
    private bool disposed = false;

    public ExecuteForScheduler(Action function, long executeForMS)
    {
        stopExecAt = Environment.TickCount64 + executeForMS;
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
        try
        {
            function();
        }
        catch(Exception e)
        {
            PluginLog.Error(e.Message + "\n" + e.StackTrace ?? "");
        }
        if(Environment.TickCount64 > stopExecAt)
        {
            Dispose();
        }
    }
}