using System;
using System.Collections.Generic;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
    /// <summary>
    /// Enqueue a single function into the end of the queue.
    /// </summary>
    public void Enqueue(Func<bool?> func, string taskName, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(func, taskName, configuration));
    /// <summary>
    /// Enqueue a single function into the end of the queue.
    /// </summary>
    public void Enqueue(Func<bool?> func, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(func, configuration));
    /// <summary>
    /// Enqueue a single function into the end of the queue.
    /// </summary>
    public void Enqueue(Func<bool> func, string taskName, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(func, taskName, configuration));
    /// <summary>
    /// Enqueue a single function into the end of the queue.
    /// </summary>
    public void Enqueue(Func<bool> func, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(func, configuration));

    /// <summary>
    /// Enqueue a single action into the end of the queue.
    /// </summary>
    public void Enqueue(Action action, string taskName, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(action, taskName, configuration));
    /// <summary>
    /// Enqueue a single action into the end of the queue.
    /// </summary>
    public void Enqueue(Action action, TaskManagerConfiguration? configuration = null) => EnqueueTask(new(action, configuration));


    /// <summary>
    /// Enqueues a single task into the end of the queue.
    /// </summary>
    /// <param name="task"></param>
    public void EnqueueTask(TaskManagerTask task) => EnqueueMulti(task);

    /// <summary>
    /// Enqueues specified tasks into the end of the queue.
    /// </summary>
    /// <param name="tasks">Sequence of tasks to enqueue</param>
    public void EnqueueMulti(params TaskManagerTask?[] tasks)
    {
        foreach (var task in tasks)
        {
            if (task == null) continue;
            if (this.IsStackActive)
						{
								Log($"(stack) Enqueued task {task.Name}", task.Configuration?.ShowDebug ?? DefaultConfiguration.ShowDebug!.Value);
								Stack.Add(task);
            }
            else
            {
                Log($"Enqueued task {task.Name}", task.Configuration?.ShowDebug ?? DefaultConfiguration.ShowDebug!.Value);
                Tasks.Add(task);
            }
        }
    }
}
