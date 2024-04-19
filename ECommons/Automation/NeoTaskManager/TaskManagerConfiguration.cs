using ECommons.Logging;
using System;

namespace ECommons.Automation.NeoTaskManager;
/// <summary>
/// Configuration class that defines TaskManager's behavior. In case task's configuration is not set or any it's property is null, default task manager configuration will be used. Default configuration is always present and can't have null properties.
/// </summary>
public class TaskManagerConfiguration
{
    public TaskManagerConfiguration(int? timeLimitMS = null, bool? abortOnTimeout = null, bool? abortOnError = null, bool? timeoutSilently = null, bool? showDebug = null, bool? showError = null, bool? executeDefaultConfigurationEvents = null)
    {
        this.TimeLimitMS = timeLimitMS;
        this.AbortOnTimeout = abortOnTimeout;
        this.AbortOnError = abortOnError;
        this.TimeoutSilently = timeoutSilently;
        this.ShowDebug = showDebug;
        this.ShowError = showError;
        this.ExecuteDefaultConfigurationEvents = executeDefaultConfigurationEvents;
    }

    /// <summary>
    /// After this amount of time task will fail. Default is 30000 (30s).
    /// </summary>
    public int? TimeLimitMS { get; set; } = null;
    /// <summary>
    /// Whether to clear entire queue of tasks if one of them times out. Otherwise, only timed out task will be discarded and the rest will continue to execute. Default is true.
    /// </summary>
    public bool? AbortOnTimeout { get; set; } = null;
		/// <summary>
		/// Whether to clear entire queue of tasks if one of them produces an exception. Otherwise, only errored task will be discarded and the rest will continue to execute. Default is true.
		/// </summary>
		public bool? AbortOnError { get; set; } = null;
    /// <summary>
    /// Whether to suppress any output when task times out. Default is false.
    /// </summary>
    public bool? TimeoutSilently { get; set; } = null;
		/// <summary>
		/// Whether to output debug information to console. Use in development or testing. Default is false.
		/// </summary>
		public bool? ShowDebug { get; set; } = null;
    /// <summary>
    /// Whether to output errors to console. Default is true.
    /// </summary>
    public bool? ShowError { get; set; } = null;
    /// <summary>
    /// Whether to always execute events that are registered in default configuration. Otherwise, default configuration's events will only be used when per-task configuration is missing. Default is true.
    /// </summary>
    public bool? ExecuteDefaultConfigurationEvents { get; set; } = null;

		/// <summary>
		/// Event that is fired when execution of a task results in an exception.
		/// </summary>
		/// <param name="task"></param>
		/// <param name="exception">An exception thrown by the task</param>
		/// <param name="continue">Whether task should continue to be executed</param>
		/// <param name="abort">Whether to change abort behavior. Keep <see langword="null"/> to apply configuration settings, change to <see langword="true"/> to abort entire operation, change to <see langword="false"/> to only discard errored task. Has no effect if <paramref name="continue"/> was set to <see langword="true"/>.</param>
		public delegate void OnTaskExceptionDelegate(TaskManagerTask task, Exception exception, ref bool @continue, ref bool? abort);
    /// <summary>
    /// Event that is fired when execution of a task results in an exception.
    /// </summary>
    public event OnTaskExceptionDelegate? OnTaskException;

		/// <summary>
		/// Event that is fired when task times out.
		/// </summary>
		/// <param name="task"></param>
		/// <param name="remainingTimeMS">Changing this option will grand a task additional time to execute.</param>
		public delegate void OnTaskTimeoutDelegate(TaskManagerTask task, ref long remainingTimeMS);
    /// <summary>
    /// Event that is fired when task times out.
    /// </summary>
    public event OnTaskTimeoutDelegate? OnTaskTimeout;

		/// <summary>
		/// Event that is fired when task is completed. Fired when task returns either true (a signal to proceed to execute next task) or null (a signal to cancel current task queue).
		/// </summary>
		/// <param name="task"></param>
		/// <param name="isCompleted">Change execution result. Changing it to false will result in task being executed again, changing it to null will cancel current task queue.</param>
		public delegate void OnTaskCompletionDelegate(TaskManagerTask task, ref bool? isCompleted);
		/// <summary>
		/// Event that is fired when task is completed. Fired when task returns either true (a signal to proceed to execute next task) or null (a signal to cancel current task queue).
		/// </summary>
		public event OnTaskCompletionDelegate? OnTaskCompletion;

		internal void FireOnTaskException(TaskManagerTask task, Exception ex, ref bool @continue, ref bool? abort)
		{
        try
        {
            OnTaskException?.Invoke(task, ex, ref @continue, ref abort);
        }
        catch(Exception e)
        {
            PluginLog.Error($"During processing {nameof(OnTaskException)} event, an exception was raised:");
            e.Log();
        }
		}

    internal void FireOnTaskTimeout(TaskManagerTask task, ref long remainingTime)
		{
				try
				{
						OnTaskTimeout?.Invoke(task, ref remainingTime);
				}
				catch (Exception e)
				{
						PluginLog.Error($"During processing {nameof(OnTaskTimeout)} event, an exception was raised:");
						e.Log();
				}
		}

		internal void FireOnTaskCompletion(TaskManagerTask task, ref bool? isCompleted)
		{
				try
				{
						OnTaskCompletion?.Invoke(task, ref isCompleted);
				}
				catch (Exception e)
				{
						PluginLog.Error($"During processing {nameof(OnTaskCompletion)} event, an exception was raised:");
						e.Log();
				}
		}

		internal void AssertNotNull()
    {
        if (this.TimeLimitMS == null) throw new NullReferenceException();
        if (this.AbortOnTimeout == null) throw new NullReferenceException();
        if (this.AbortOnError == null) throw new NullReferenceException();
        if (this.TimeoutSilently == null) throw new NullReferenceException();
        if (this.ShowDebug == null) throw new NullReferenceException();
        if (this.ShowError == null) throw new NullReferenceException();
        if (this.ExecuteDefaultConfigurationEvents == null) throw new NullReferenceException();
    }

		/// <summary>
		/// Produces copy of other configuration and fills it's null properties from current configuration.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="copyEvents">Whether to copy events. If true, events from other current configuration will be used, if false - from other.</param>
		/// <returns></returns>
		public TaskManagerConfiguration With(TaskManagerConfiguration? other, bool copyEvents = true)
    {
        return new()
        {
            TimeLimitMS = other?.TimeLimitMS ?? this.TimeLimitMS,
            AbortOnError = other?.AbortOnError ?? this.AbortOnError,
            AbortOnTimeout = other?.AbortOnTimeout ?? this.AbortOnTimeout,
            ShowDebug = other?.ShowDebug ?? this.ShowDebug,
            ShowError = other?.ShowError ?? this.ShowError,
						TimeoutSilently = other?.TimeoutSilently ?? this.TimeoutSilently,
						ExecuteDefaultConfigurationEvents = other?.ExecuteDefaultConfigurationEvents ?? this.ExecuteDefaultConfigurationEvents,

						OnTaskCompletion = copyEvents ? (other?.OnTaskCompletion) : OnTaskCompletion,
						OnTaskTimeout = copyEvents ? (other?.OnTaskTimeout) : OnTaskTimeout,
						OnTaskException = copyEvents ? (other?.OnTaskException) : OnTaskException,
				};
    }
}
