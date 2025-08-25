namespace ECommons.Schedulers.SingleThreadRunnerHelper;

using ECommons.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SingleThreadContext : IDisposable
{
    private readonly BlockingCollection<(SendOrPostCallback, object)> queue = [];

    public SingleThreadContext()
    {
        var syncContext = new InternalSyncContext(queue);
        SynchronizationContext.SetSynchronizationContext(syncContext);
    }

    public void Dispose()
    {
        queue.CompleteAdding();
    }

    public void Run(Action action)
    {
        Run(async () =>
        {
            action();
            await Task.CompletedTask;
        });
    }

    public T Run<T>(Func<T> func)
    {
        return Run(async () => await Task.FromResult(func()));
    }

    public void Run(Func<Task> func)
    {
        Run(async () =>
        {
            await func();
            return true;
        });
    }

    public T Run<T>(Func<Task<T>> func)
    {
        T result = default!;
        Exception exception = null;
        var done = new ManualResetEventSlim(false);

        SynchronizationContext.Current!.Post(async _ =>
        {
            try
            {
                result = await func().ConfigureAwait(true);
            }
            catch(Exception ex)
            {
                exception = ex;
            }
            finally
            {
                done.Set();
            }
        }, null);

        done.Wait();

        if(exception != null)
        {
            PluginLog.Error("Exception in SingleThreadContext.Run: " + exception);
            return default!;
        }

        return result;
    }

    private sealed class InternalSyncContext : SynchronizationContext
    {
        private readonly BlockingCollection<(SendOrPostCallback, object)> queue;

        public InternalSyncContext(BlockingCollection<(SendOrPostCallback, object)> queue)
        {
            this.queue = queue;
            var pumpThread = new Thread(RunLoop) { IsBackground = true };
            pumpThread.Start();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if(ThreadGuard.IsSingleThread)
            {
                PluginLog.Error("Error: \"await\" used inside SingleThreadRunner. Use .Sync() instead.");
            }
            queue.Add((d, state));
        }

        private void RunLoop()
        {
            foreach(var (callback, state) in queue.GetConsumingEnumerable())
            {
                try
                {
                    callback(state);
                }
                catch(Exception ex)
                {
                    PluginLog.Error("Exception in SingleThreadRunner continuation: " + ex);
                }
            }
        }
    }
}
