using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ECommons.Schedulers.Sequential.TaskQueue;

[Experimental("E1")]
public partial class SequentialTaskQueue
{
    private class QueuedTask
    {
        private readonly Action _action;
        private readonly List<Action> _completionCallbacks = [];

        public QueuedTask(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }

        public void OnCompletion(Action callback)
        {
            _completionCallbacks.Add(callback);
        }

        public void MarkDiscarded()
        {
            foreach(var callback in _completionCallbacks)
            {
                try
                {
                    callback();
                }
                catch(Exception ex)
                {
                    ex.Log();
                }
            }
        }
    }
}