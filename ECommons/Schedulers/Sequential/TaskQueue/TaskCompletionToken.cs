using System;

namespace ECommons.Schedulers.Sequential.TaskQueue;

public partial class SequentialTaskQueue
{
    public class TaskCompletionToken<T> : TaskCompletionToken
    {
        public T Result { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}