using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public unsafe class FrameDelayTask
{
    private long? StopAt;
    internal TaskManagerTask Task;
    public FrameDelayTask(int ms)
    {
        Task = new((Func<bool?>)(() =>
        {
            StopAt ??= Framework.Instance()->FrameCounter + ms;
            return Framework.Instance()->FrameCounter >= StopAt;
        }), $"Delay ({ms} frames)", new(abortOnTimeout: false));
    }

    public static implicit operator TaskManagerTask(FrameDelayTask task) => task.Task;
}
