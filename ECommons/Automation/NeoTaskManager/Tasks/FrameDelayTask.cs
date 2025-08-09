using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public unsafe class FrameDelayTask
{
    private long? StopAt;
    internal TaskManagerTask Task;
    public FrameDelayTask(int ms, TaskManagerConfiguration? configuration = null)
    {
        var config = new TaskManagerConfiguration(abortOnTimeout: false);
        if(configuration != null)
        {
            config = config.With(configuration);
        }
        Task = new((Func<bool?>)(() =>
        {
            StopAt ??= Framework.Instance()->FrameCounter + ms;
            return Framework.Instance()->FrameCounter >= StopAt;
        }), $"Delay ({ms} frames)", config);
    }

    public static implicit operator TaskManagerTask(FrameDelayTask task) => task.Task;
}
