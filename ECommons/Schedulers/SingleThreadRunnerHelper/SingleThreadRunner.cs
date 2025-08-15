using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ECommons.Schedulers.SingleThreadRunnerHelper;

public sealed class SingleThreadRunner
{
    private readonly Thread thread;
    private volatile bool running;

    public bool IsRunning => running;

    public SingleThreadRunner(Action action, string name = "SingleThreadRunner")
    {
        thread = new Thread(() =>
        {
            using var ctx = new SingleThreadContext();
            ThreadGuard.IsSingleThread = true;
            SyncHelper.SetContext(ctx);

            try
            {
                running = true;
                action();
            }
            catch(Exception ex)
            {
                PluginLog.Error("Unhandled exception in SingleThreadRunner: " + ex);
            }
            finally
            {
                running = false;
                ThreadGuard.IsSingleThread = false;
                SyncHelper.SetContext(null);
            }
        })
        {
            IsBackground = true,
            Name = name
        };
    }

    public void Start() => thread.Start();

    public void Join() => thread.Join();
}
