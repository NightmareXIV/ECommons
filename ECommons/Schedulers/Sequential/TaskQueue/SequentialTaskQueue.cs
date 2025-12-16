using ECommons.LazyDataHelpers;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ECommons.Schedulers.Sequential.TaskQueue;

public partial class SequentialTaskQueue : IDisposable
{
    internal static List<SequentialTaskQueue> Queues
    {
        get
        {
            if(field == null)
            {
                Purgatory.Add(() =>
                {
                    var queues = field;
                    field = null;
                    foreach(var f in field)
                    {
                        try
                        {
                            f.Dispose();
                        }
                        catch(Exception e)
                        {
                            e.Log();
                        }
                    }
                });
                field = [];
            }
            return field;
        }
    }

    private readonly Queue<QueuedTask> _taskQueue = new();
    private readonly object _lock = new();
    private readonly Action<Exception> _exceptionHandler;
    private readonly int _idleTimeoutMs;

    private Thread _workerThread;
    private bool _isDisposed;
    private long _lastTaskTime;
    private volatile bool _shouldStop;

    public SequentialTaskQueue(Action<Exception> exceptionHandler, int idleTimeoutSeconds = 60)
    {
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        _idleTimeoutMs = idleTimeoutSeconds * 1000;
        _lastTaskTime = Environment.TickCount64;
    }

    public TaskCompletionToken<T> Enqueue<T>(Func<T> task, bool clearQueue = false)
    {
        var token = new TaskCompletionToken<T>();
        var queuedTask = new QueuedTask(() =>
        {
            try
            {
                token.Result = task();
                token.MarkCompleted();
            }
            catch(Exception ex)
            {
                token.Exception = ex;
                token.MarkCompleted();
                SafeInvokeExceptionHandler(ex);
            }
        });

        EnqueueInternal(queuedTask, clearQueue);
        return token;
    }

    public TaskCompletionToken Enqueue(Action task, Action<Exception> callback = null, bool clearQueue = false)
    {
        var token = new TaskCompletionToken();
        var queuedTask = new QueuedTask(() =>
        {
            Exception caughtException = null;
            try
            {
                task();
            }
            catch(Exception ex)
            {
                caughtException = ex;
                SafeInvokeExceptionHandler(ex);
            }
            finally
            {
                token.MarkCompleted();
                if(callback != null)
                {
                    try
                    {
                        callback(caughtException);
                    }
                    catch(Exception callbackEx)
                    {
                        SafeInvokeExceptionHandler(callbackEx);
                    }
                }
            }
        });

        EnqueueInternal(queuedTask, clearQueue);
        return token;
    }

    private void EnqueueInternal(QueuedTask task, bool clearQueue)
    {
        lock(_lock)
        {
            if(_isDisposed)
            {
                task.MarkDiscarded();
                return;
            }

            if(clearQueue)
            {
                while(_taskQueue.Count > 0)
                {
                    var discarded = _taskQueue.Dequeue();
                    discarded.MarkDiscarded();
                }
            }

            _taskQueue.Enqueue(task);
            _lastTaskTime = Environment.TickCount64;

            if(_workerThread == null || !_workerThread.IsAlive)
            {
                _shouldStop = false;
                _workerThread = new Thread(WorkerThreadProc)
                {
                    IsBackground = true,
                    Name = "SequentialTaskQueue Worker"
                };
                _workerThread.Start();
            }

            Monitor.Pulse(_lock);
        }
    }

    private void WorkerThreadProc()
    {
        while(true)
        {
            QueuedTask task = null;

            lock(_lock)
            {
                while(_taskQueue.Count == 0 && !_shouldStop)
                {
                    var timeElapsed = Environment.TickCount64 - _lastTaskTime;
                    var timeRemaining = _idleTimeoutMs - (int)timeElapsed;

                    if(timeRemaining <= 0)
                    {
                        return; 
                    }

                    if(!Monitor.Wait(_lock, timeRemaining))
                    {
                        if(_taskQueue.Count == 0 && !_shouldStop)
                        {
                            return; 
                        }
                    }
                }

                if(_shouldStop && _taskQueue.Count == 0)
                {
                    return;
                }

                if(_taskQueue.Count > 0)
                {
                    task = _taskQueue.Dequeue();
                }
            }

            if(task != null)
            {
                try
                {
                    task.Execute();
                }
                catch(Exception ex)
                {
                    SafeInvokeExceptionHandler(ex);
                }
            }
        }
    }

    private void SafeInvokeExceptionHandler(Exception ex)
    {
        try
        {
            _exceptionHandler?.Invoke(ex);
        }
        catch(Exception ex2)
        {
            ex2.Log();
        }
    }

    public void Dispose()
    {
        lock(_lock)
        {
            if(_isDisposed)
                return;

            _isDisposed = true;
            _shouldStop = true;

            while(_taskQueue.Count > 0)
            {
                var task = _taskQueue.Dequeue();
                task.MarkDiscarded();
            }

            Monitor.PulseAll(_lock);
        }
    }
}