using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommons.Automation.NeoTaskManager;
public class TaskManagerTask
{
    public readonly string Name;
    public readonly Func<bool?> Function;
    public readonly TaskManagerConfiguration? Configuration;

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
