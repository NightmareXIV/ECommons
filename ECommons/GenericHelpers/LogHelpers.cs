using ECommons.Logging;
using System;
using System.Text;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    public static string ToStringFull(this Exception e)
    {
        var str = new StringBuilder($"{e.Message}\n{e.StackTrace}");
        var inner = e.InnerException;
        for(var i = 1; inner != null; i++)
        {
            str.Append($"\nAn inner exception ({i}) was thrown: {e.Message}\n{e.StackTrace}");
            inner = inner.InnerException;
        }
        return str.ToString();
    }

    public static void Log(this Exception e, Action<string> exceptionFunc)
    {
        exceptionFunc(e.ToStringFull());
    }

    public static void Log(this Exception e) => e.Log(PluginLog.Error);
    public static void Log(this Exception e, string ErrorMessage)
    {
        PluginLog.Error($"{ErrorMessage}");
        e.Log(PluginLog.Error);
    }

    public static void LogFatal(this Exception e) => e.Log(PluginLog.Fatal);
    public static void LogFatal(this Exception e, string ErrorMessage)
    {
        PluginLog.Fatal($"{ErrorMessage}");
        e.Log(PluginLog.Fatal);
    }

    public static void LogWarning(this Exception e) => e.Log(PluginLog.Warning);
    public static void LogWarning(this Exception e, string errorMessage)
    {
        PluginLog.Warning(errorMessage);
        e.Log(PluginLog.Warning);
    }

    public static void LogVerbose(this Exception e) => e.Log(PluginLog.Verbose);
    public static void LogVerbose(this Exception e, string ErrorMessage)
    {
        PluginLog.Verbose(ErrorMessage);
        e.Log(PluginLog.Verbose);
    }

    public static void LogInternal(this Exception e) => e.Log(InternalLog.Error);
    public static void LogInternal(this Exception e, string ErrorMessage)
    {
        InternalLog.Error(ErrorMessage);
        e.Log(InternalLog.Error);
    }

    public static void LogDebug(this Exception e) => e.Log(PluginLog.Debug);
    public static void LogDebug(this Exception e, string ErrorMessage)
    {
        PluginLog.Debug(ErrorMessage);
        e.Log(PluginLog.Debug);
    }

    public static void LogInfo(this Exception e) => e.Log(PluginLog.Information);
    public static void LogInfo(this Exception e, string ErrorMessage)
    {
        PluginLog.Information(ErrorMessage);
        e.Log(PluginLog.Information);
    }
    public static void LogDuo(this Exception e) => e.Log(DuoLog.Error);
}
