using System;

namespace ECommons.Automation.NeoTaskManager;
public class TaskManagerConfiguration
{
    public TaskManagerConfiguration(int? timeLimitMS = null, bool? abortOnTimeout = null, bool? abortOnError = null, bool? timeoutSilently = null, bool? showDebug = null, bool? showError = null)
    {
        this.TimeLimitMS = timeLimitMS;
        this.AbortOnTimeout = abortOnTimeout;
        this.AbortOnError = abortOnError;
        this.TimeoutSilently = timeoutSilently;
        this.ShowDebug = showDebug;
        this.ShowError = showError;
    }

    public int? TimeLimitMS { get; set; } = null;
    public bool? AbortOnTimeout { get; set; } = null;
    public bool? AbortOnError { get; set; } = null;
    public bool? TimeoutSilently { get; set; } = null;
    public bool? ShowDebug { get; set; } = null;
    public bool? ShowError { get; set; } = null;

    internal void AssertNotNull()
    {
        if (this.TimeLimitMS == null) throw new NullReferenceException();
        if (this.AbortOnTimeout == null) throw new NullReferenceException();
        if (this.AbortOnError == null) throw new NullReferenceException();
        if (this.TimeoutSilently == null) throw new NullReferenceException();
        if (this.ShowDebug == null) throw new NullReferenceException();
        if (this.ShowError == null) throw new NullReferenceException();
    }

    public TaskManagerConfiguration With(TaskManagerConfiguration other)
    {
        return new()
        {
            TimeLimitMS = other?.TimeLimitMS ?? this.TimeLimitMS,
            AbortOnError = other?.AbortOnError ?? this.AbortOnError,
            AbortOnTimeout = other?.AbortOnTimeout ?? this.AbortOnTimeout,
            ShowDebug = other?.ShowDebug ?? this.ShowDebug,
            ShowError = other?.ShowError ?? this.ShowError,
            TimeoutSilently = other?.TimeoutSilently ?? this.TimeoutSilently,
        };
    }
}
