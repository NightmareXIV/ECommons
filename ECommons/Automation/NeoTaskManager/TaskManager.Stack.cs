using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
    public record struct StackDetails()
    {
        public string Name { get; set; }
        public List<TaskManagerTask> Stack { get; set; } = [];
    }
    /// <summary>
    /// Provides temporary dedicated storage for tasks where tasks are put for future use.
    /// </summary>
    public List<StackDetails> Stack { get; init; } = [];

    /// <summary>
    /// Whether stack mode is active. When active, newly enqueued and inserted tasks go into stack instead of main queue.
    /// </summary>
    public bool IsStackActive => Stack.Count > 0;

    /// <summary>
    /// Enables stack mode. Euqueue and Insert calls will go into the stack instead of queue after this call.
    /// </summary>
    [Obsolete($"Use {nameof(EnqueueStack)} with action")]
    public void BeginStack(string? name)
    {
        name ??= "Unnamed stack";
        Stack.Add(new() { Name = name });
        if(DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Stack mode begins (cnt: {Stack.Count})");
    }

    /// <inheritdoc cref="BeginStack(string)"/>
    [Obsolete($"Use {nameof(EnqueueStack)} with action")]
    public void BeginStack() => BeginStack(null);

    /// <summary>
    /// Enqueues the whole stack of tasks into the end of primary queue, disables stack mode and clears the stack afterwards.
    /// </summary>
    [Obsolete($"Use {nameof(EnqueueStack)} with action")]
    public void EnqueueStack()
    {
        if(Stack.TryDequeueLast(out var stack))
        {
            if(DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Enqueueing stack {stack.Name} with {stack.Stack.Count} tasks");
            foreach(var item in stack.Stack)
            {
                item.Name = $"[Stack (enqueue, {stack.Stack.Count}): {stack.Name}] {item.Name}";
            }
            EnqueueMulti([.. stack.Stack]);
        }
        else
        {
            PluginLog.Error($"Could not find a stack to enqueue.");
        }
    }

    [Obsolete($"Use {nameof(EnqueueStack)} with action")]
    /// <summary>
    /// Inserts the whole stack of tasks into the beginning of primary queue, disables stack mode and clears the stack afterwards.
    /// </summary>
    public void InsertStack()
    {
        if(Stack.TryDequeueLast(out var stack))
        {
            if(DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Inserting stack {stack.Name} with {stack.Stack.Count} tasks");
            foreach(var item in stack.Stack)
            {
                item.Name = $"[Stack (insert, {stack.Stack.Count}): {stack.Name}] {item.Name}";
            }
            InsertMulti([.. stack.Stack]);
        }
        else
        {
            PluginLog.Error($"Could not find a stack to insert.");
        }
    }

    /// <summary>
    /// Disables stack mode and clears the stack.
    /// </summary>
    [Obsolete($"Use {nameof(EnqueueStack)} with action")]
    public void DiscardStack()
    {
        if(Stack.TryDequeueLast(out var stack))
        {
            if(DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Discarding stack with {stack} tasks");
        }
    }

    /// <Inheritdoc cref="EnqueueStack(string, Action)"/>
    public void EnqueueStack(Action executeAction) => EnqueueStack(null, executeAction);

    /// <summary>
    /// Immediately executes <paramref name="executeAction"/> in between <see cref="BeginStack"/> and <see cref="EnqueueStack"/>. Handles exceptions. If an exception is thrown, will reset discard instead. 
    /// </summary>
    /// <param name="executeAction"></param>
    public void EnqueueStack(string name, Action executeAction)
    {
        BeginStack(name);
        try
        {
            executeAction();
        }
        catch(Exception e)
        {
            e.Log();
            DiscardStack();
            return;
        }
        EnqueueStack();
    }

    /// <Inheritdoc cref="InsertStack(string, Action)"/>
    public void InsertStack(Action executeAction) => InsertStack(null, executeAction);

    /// <summary>
    /// Immediately executes <paramref name="executeAction"/> in between <see cref="BeginStack"/> and <see cref="InsertStack"/>. Handles exceptions. If an exception is thrown, will reset discard instead. 
    /// </summary>
    /// <param name="executeAction"></param>
    public void InsertStack(string name, Action executeAction)
    {
        BeginStack(name);
        try
        {
            executeAction();
        }
        catch(Exception e)
        {
            e.Log();
            DiscardStack();
            return;
        }
        InsertStack();
    }
}
