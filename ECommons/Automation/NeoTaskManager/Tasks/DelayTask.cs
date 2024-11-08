using System;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public class DelayTask
{
    private long? StopAt;
    internal TaskManagerTask Task;
    public DelayTask(int ms)
    {
        Task = new((Func<bool?>)(() =>
        {
            StopAt ??= Environment.TickCount64 + ms;
            return Environment.TickCount64 >= StopAt;
        }), $"Delay ({ms}ms)", new(timeLimitMS: ms * 2 + 5000, abortOnTimeout: false));
    }

    public static implicit operator TaskManagerTask(DelayTask task) => task.Task;
}
