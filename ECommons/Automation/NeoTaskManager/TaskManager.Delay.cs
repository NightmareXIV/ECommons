using ECommons.Automation.NeoTaskManager.Tasks;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
    /// <summary>
    /// Direct replacement of DelayNext
    /// </summary>
    /// <param name="ms">Amount of delay</param>
    /// <param name="isFrame">If true, delay is measured in seconds and not </param>
    public void EnqueueDelay(int ms, bool isFrame = false)
    {
        if (isFrame)
        {
            EnqueueTask(new FrameDelayTask(ms));
        }
        else
        {
            EnqueueTask(new DelayTask(ms));
        }
    }

    /// <summary>
    /// Direct replacement of DelayNextImmediate
    /// </summary>
    /// <param name="ms">Amount of delay</param>
    /// <param name="isFrame">If true, delay is measured in seconds and not </param>
    public void InsertDelay(int ms, bool isFrame = false)
    {
        if (isFrame)
        {
            InsertTask(new FrameDelayTask(ms));
        }
        else
        {
            InsertTask(new DelayTask(ms));
        }
    }
}
