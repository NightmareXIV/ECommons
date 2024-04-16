using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public class DelayTask
{
    long StopAt;
    internal TaskManagerTask Task;
    public DelayTask(int ms)
    {
        StopAt = Environment.TickCount64 + ms;
        Task = new((Func<bool?>)(() => Environment.TickCount64 >= StopAt), $"Delay ({ms}ms)", new(timeLimitMS: ms * 2));
    }

    public static implicit operator TaskManagerTask(DelayTask task) => task.Task;
}
