using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
    /// <summary>
    /// Insert a single function into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Func<bool?> func, string taskName, TaskManagerConfiguration? configuration = null) => InsertTask(new(func, taskName, configuration));
    /// <summary>
    /// Insert a single function into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Func<bool?> func, TaskManagerConfiguration? configuration = null) => InsertTask(new(func, configuration));
    /// <summary>
    /// Insert a single function into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Func<bool> func, string taskName, TaskManagerConfiguration? configuration = null) => InsertTask(new(func, taskName, configuration));
    /// <summary>
    /// Insert a single function into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Func<bool> func, TaskManagerConfiguration? configuration = null) => InsertTask(new(func, configuration));

    /// <summary>
    /// Insert a single action into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Action action, string taskName, TaskManagerConfiguration? configuration = null) => InsertTask(new(action, taskName, configuration));
    /// <summary>
    /// Insert a single action into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    public void Insert(Action action, TaskManagerConfiguration? configuration = null) => InsertTask(new(action, configuration));

    /// <summary>
    /// Inserts a single task into the beginning of the queue. Warning! You can not replace EnqueueImmediate calls from legacy task manager with this. Insert an entire range of tasks instead.
    /// </summary>
    /// <param name="task"></param>
    public void InsertTask(TaskManagerTask task) => InsertMulti(task);

    /// <summary>
    /// Inserts specified tasks into the beginning of the queue, preserving the order. Is similar to Legacy Task Manager's InsertImmediate, except you can have unlimited nested calls.
    /// </summary>
    /// <param name="tasks">Sequence of tasks to Insert</param>
    public void InsertMulti(params TaskManagerTask?[] tasks)
    {
        foreach (var task in tasks.Reverse())
        {
            if (task == null) continue;
            if (IsStackActive)
						{
								Log($"(stack) Inserted task {task.Name}", task.Configuration?.ShowDebug ?? DefaultConfiguration.ShowDebug!.Value);
								Stack.Insert(0, task);
            }
            else
            {
                Log($"Inserted task {task.Name}", task.Configuration?.ShowDebug ?? DefaultConfiguration.ShowDebug!.Value);
                Tasks.Insert(0, task);
            }
        }
    }
}
