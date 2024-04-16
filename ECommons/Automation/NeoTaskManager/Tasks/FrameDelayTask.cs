using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.NeoTaskManager.Tasks;
public unsafe class FrameDelayTask
{
    long StopAt;
    internal TaskManagerTask Task;
    public FrameDelayTask(int ms)
    {
        StopAt = Framework.Instance()->FrameCounter + ms;
        Task = new((Func<bool?>)(() => Framework.Instance()->FrameCounter >= StopAt), $"Delay ({ms} frames)");
    }

    public static implicit operator TaskManagerTask(FrameDelayTask task) => task.Task;
}
