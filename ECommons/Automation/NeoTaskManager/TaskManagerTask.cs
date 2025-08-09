using System;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommons.Automation.NeoTaskManager;
public class TaskManagerTask
{
    public string Name { get; init; }
    public string Location { get; init; }
    public Func<bool?> Function { get; init; }
    public TaskManagerConfiguration? Configuration { get; init; }

    public TaskManagerTask(Func<bool?> function, TaskManagerConfiguration? configuration = null)
    {
        Function = function;
        Configuration = configuration;
        Name = function.GetMethodInfo().Name ?? "";
        Location = function.GetMethodInfo().DeclaringType?.Name ?? "";
    }

    public TaskManagerTask(Func<bool> function, TaskManagerConfiguration? configuration = null)
    {
        Function = () => function();
        Configuration = configuration;
        Name = function.GetMethodInfo().Name ?? "";
        Location = function.GetMethodInfo().DeclaringType?.Name ?? "";
    }

    public TaskManagerTask(Action action, TaskManagerConfiguration? configuration = null)
    {
        Function = () =>
        {
            action();
            return true;
        };
        Configuration = configuration;
        Name = action.GetMethodInfo().Name ?? "";
        Location = action.GetMethodInfo().DeclaringType?.Name ?? "";
    }

    public TaskManagerTask(Func<bool?> function, string taskName, TaskManagerConfiguration? configuration = null)
    {
        Function = function;
        Configuration = configuration;
        Name = taskName;
        Location = function.GetMethodInfo().DeclaringType?.Name ?? "";
    }

    public TaskManagerTask(Func<bool> function, string taskName, TaskManagerConfiguration? configuration = null)
    {
        Function = () => function();
        Configuration = configuration;
        Name = taskName;
        Location = function.GetMethodInfo().DeclaringType?.Name ?? "";
    }

    public TaskManagerTask(Action action, string taskName, TaskManagerConfiguration? configuration = null)
    {
        Function = () =>
        {
            action();
            return true;
        };
        Configuration = configuration;
        Name = taskName;
        Location = action.GetMethodInfo().DeclaringType?.Name ?? "";
    }
}
