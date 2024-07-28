using ECommons.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommons.Schedulers;
/// <summary>
/// A helper to let you have your own simple thread pool.
/// </summary>
public class ThreadPool : IDisposable
{
    private readonly int MaxThreads = 8;
    private ConcurrentQueue<(Action Action, Action<Exception?>? OnCompletion)> TaskQueue = [];
    private volatile uint ThreadNum = 0;
    private volatile uint BusyThreads = 0;
    private volatile bool Disposed = false;
    public (uint RunningThreads, uint BusyThreads, int TasksQueued) State => (ThreadNum, BusyThreads, TaskQueue.Count);
    public ThreadPool()
    {
    }

    public ThreadPool(int maxThreads)
    {
        MaxThreads = maxThreads;
    }

    public void Dispose()
    {
        Disposed = true;
    }

    /// <summary>
    /// Enqueues an action to be ran in one of threads. If amount of currently running threads will be less than MaxThreads, new thread will be created, otherwise action will be picked by first free thread.
    /// </summary>
    /// <param name="task">An action to run.</param>
    /// <param name="onCompletion">If specified, will be executed after completion of main action. If specified, no errors will be printed in log, instead if any occurs it will be passed into an action as an argument.</param>
    public void Run(Action task, Action<Exception?>? onCompletion = null)
    {
        TaskQueue.Enqueue((task, onCompletion));
        var num = Math.Max(1, Math.Min(MaxThreads, TaskQueue.Count + BusyThreads));
        if(ThreadNum < num)
        {
            PluginLog.Verbose($"{ThreadNum} threads running, {BusyThreads} are busy, requested {num} threads, Creating new thread to deal with tasks...");
            ThreadNum++;
            new Thread(ThreadRun).Start();
        }
        else
        {
            //PluginLog.Verbose($"{num} threads already running, no new!");
        }
    }

    private void ThreadRun()
    {
        var uniqueID = $"{Random.Shared.Next():X8}";
        PluginLog.Verbose($"Thread {uniqueID} begins!");
        var idleTicks = 0;
        while(!Disposed)
        {
            if(TaskQueue.TryDequeue(out var result))
            {
                BusyThreads++;
                idleTicks = 0;
                Exception? exc = null;
                try
                {
                    result.Action();
                }
                catch(Exception ex)
                {
                    if(result.OnCompletion == null)
                    {
                        ex.Log();
                    }
                    else
                    {
                        exc = ex;
                    }
                }
                if(result.OnCompletion != null)
                {
                    try
                    {
                        result.OnCompletion(exc);
                    }
                    catch(Exception ex)
                    {
                        ex.Log();
                    }
                }
                BusyThreads--;
            }
            else
            {
                idleTicks++;
                Thread.Sleep(100);
                if(idleTicks > 100 || Disposed)
                {
                    ThreadNum--;
                    break;
                }
            }
        }
        PluginLog.Verbose($"Thread {uniqueID} ends!");
    }
}
