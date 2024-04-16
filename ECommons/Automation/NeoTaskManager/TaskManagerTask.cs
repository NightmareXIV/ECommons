using System;
using System.Reflection;

namespace ECommons.Automation.NeoTaskManager;
public class TaskManagerTask
{
    public string Name { get; init; }
    public Func<bool?> Function { get; init; }
    public TaskManagerConfiguration? Configuration { get; init; }

    public TaskManagerTask(Func<bool?> function, TaskManagerConfiguration? configuration = null)
    {
        this.Function = function;
        this.Configuration = configuration;
        this.Name = function.GetMethodInfo().Name ?? ""; 
    }

    public TaskManagerTask(Func<bool> function, TaskManagerConfiguration? configuration = null)
    {
        this.Function = () => function();
        this.Configuration = configuration;
        this.Name = function.GetMethodInfo().Name ?? "";
    }

    public TaskManagerTask(Action action, TaskManagerConfiguration? configuration = null)
    {
        this.Function = () =>
        {
            action();
            return true;
        };
        this.Configuration = configuration;
        this.Name = action.GetMethodInfo().Name ?? "";
    }

    public TaskManagerTask(Func<bool?> function, string taskName, TaskManagerConfiguration? configuration = null)
    {
        this.Function = function;
        this.Configuration = configuration;
        this.Name = taskName;
    }

    public TaskManagerTask(Func<bool> function, string taskName, TaskManagerConfiguration? configuration = null)
    {
        this.Function = () => function();
        this.Configuration = configuration;
        this.Name = taskName;
    }

    public TaskManagerTask(Action action, string taskName, TaskManagerConfiguration? configuration = null)
    {
        this.Function = () =>
        {
            action();
            return true;
        };
        this.Configuration = configuration;
        this.Name = taskName;
    }
}
