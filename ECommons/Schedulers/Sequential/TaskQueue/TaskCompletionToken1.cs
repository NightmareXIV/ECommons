using System.Threading;

namespace ECommons.Schedulers.Sequential.TaskQueue;

public partial class SequentialTaskQueue
{
    public class TaskCompletionToken
    {
        private volatile bool _isCompleted;
        private readonly ManualResetEventSlim _completionEvent = new(false);

        public bool IsCompleted => _isCompleted;

        internal void MarkCompleted()
        {
            _isCompleted = true;
            _completionEvent.Set();
        }

        public void Wait()
        {
            _completionEvent.Wait();
        }

        public bool Wait(int millisecondsTimeout)
        {
            return _completionEvent.Wait(millisecondsTimeout);
        }
    }
}