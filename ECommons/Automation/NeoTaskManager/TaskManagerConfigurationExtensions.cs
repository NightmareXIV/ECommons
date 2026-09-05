namespace ECommons.Automation.NeoTaskManager;

public static class TaskManagerConfigurationExtensions
{
    extension(TaskManagerConfiguration value)
    {
        /// <summary>
        /// Produces copy of other configuration and fills it's null properties from current configuration.
        /// </summary>
        /// <param name="dominantConfiguration">Dominant configuration</param>
        /// <param name="copyEvents">Whether to copy events. If true, events from other current configuration will be used, if false - from other.</param>
        /// <returns></returns>
        public TaskManagerConfiguration With(TaskManagerConfiguration? dominantConfiguration, bool copyEvents = true)
        {
            value ??= new();
            var ret = new TaskManagerConfiguration()
            {
                TimeLimitMS = dominantConfiguration?.TimeLimitMS ?? value.TimeLimitMS,
                AbortOnError = dominantConfiguration?.AbortOnError ?? value.AbortOnError,
                AbortOnTimeout = dominantConfiguration?.AbortOnTimeout ?? value.AbortOnTimeout,
                ShowDebug = dominantConfiguration?.ShowDebug ?? value.ShowDebug,
                ShowError = dominantConfiguration?.ShowError ?? value.ShowError,
                TimeoutSilently = dominantConfiguration?.TimeoutSilently ?? value.TimeoutSilently,
                ExecuteDefaultConfigurationEvents = dominantConfiguration?.ExecuteDefaultConfigurationEvents ?? value.ExecuteDefaultConfigurationEvents,
            };
            if(copyEvents)
            {
                ret.OnTaskCompletion = dominantConfiguration?.OnTaskCompletion;
                ret.OnTaskTimeout = (dominantConfiguration?.OnTaskTimeout);
                ret.OnTaskException = (dominantConfiguration?.OnTaskException);
                ret.CompanionAction = (dominantConfiguration?.CompanionAction);
            }
            else
            {
                ret.OnTaskCompletion = (value.OnTaskCompletion);
                ret.OnTaskTimeout = (value.OnTaskTimeout);
                ret.OnTaskException = (value.OnTaskException);
                ret.CompanionAction = (value.CompanionAction);
            }
            return ret;
        }
    }
}