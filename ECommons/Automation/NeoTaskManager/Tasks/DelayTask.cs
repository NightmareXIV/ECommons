using ECommons.Reflection;
using System;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public class DelayTask
{
    private long? StopAt;
    internal TaskManagerTask Task;
    public DelayTask(int ms, TaskManagerConfiguration? configuration = null)
    {
        var config = new TaskManagerConfiguration(timeLimitMS: ms * 2 + 5000, abortOnTimeout: false);
        if(configuration != null)
        {
            config = config.With(configuration);
        }
        Task = new((Func<bool?>)(() =>
        {
            StopAt ??= Environment.TickCount64 + ms;
            return Environment.TickCount64 >= StopAt;
        }), $"Delay ({ms}ms)", config);
    }

    public static implicit operator TaskManagerTask(DelayTask task) => task.Task;
}
