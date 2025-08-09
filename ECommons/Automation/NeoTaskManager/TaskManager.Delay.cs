using ECommons.Automation.NeoTaskManager.Tasks;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
    /// <summary>
    /// Direct replacement of DelayNext
    /// </summary>
    /// <param name="ms">Amount of delay</param>
    /// <param name="isFrame">If true, delay is measured in seconds and not </param>
    /// <param name="configuration"></param>
    public void EnqueueDelay(int ms, bool isFrame = false, TaskManagerConfiguration configuration = null)
    {
        if(isFrame)
        {
            EnqueueTask(new FrameDelayTask(ms, configuration));
        }
        else
        {
            EnqueueTask(new DelayTask(ms, configuration));
        }
    }

    /// <summary>
    /// Direct replacement of DelayNextImmediate
    /// </summary>
    /// <param name="ms">Amount of delay</param>
    /// <param name="isFrame">If true, delay is measured in seconds and not </param>
    public void InsertDelay(int ms, bool isFrame = false, TaskManagerConfiguration configuration = null)
    {
        if(isFrame)
        {
            InsertTask(new FrameDelayTask(ms, configuration));
        }
        else
        {
            InsertTask(new DelayTask(ms, configuration));
        }
    }
}
