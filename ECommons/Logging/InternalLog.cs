using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using ImGuiNET;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Logging
{
    public class InternalLog
    {
        public static readonly CircularArray<InternalLogMessage> Messages = new(1000);
        public static void Information(string s)
        {
            Messages.Push(new(s, LogEventLevel.Information));
        }
        public static void Error(string s)
        {
            Messages.Push(new(s, LogEventLevel.Error));
        }
        public static void Fatal(string s)
        {
            Messages.Push(new(s, LogEventLevel.Fatal));
        }
        public static void Debug(string s)
        {
            Messages.Push(new(s, LogEventLevel.Debug));
        }
        public static void Verbose(string s)
        {
            Messages.Push(new(s, LogEventLevel.Verbose));
        }
        public static void Warning(string s)
        {
            Messages.Push(new(s, LogEventLevel.Warning));
        }
        public static void LogInformation(string s)
        {
            Information(s);
        }
        public static void LogError(string s)
        {
            Error(s);
        }
        public static void LogFatal(string s)
        {
            Fatal(s);
        }
        public static void LogDebug(string s)
        {
            Debug(s);
        }
        public static void LogVerbose(string s)
        {
            Verbose(s);
        }
        public static void LogWarning(string s)
        {
            Warning(s);
        }
        public static void Log(string s)
        {
            Information(s);
        }

        public static void PrintImgui()
        {
            ImGui.BeginChild($"Plugin_log{DalamudReflector.GetPluginName()}");
            foreach(var x in Messages)
            {
                ImGuiEx.TextWrappedCopy(x.Level == LogEventLevel.Fatal?ImGuiColors.DPSRed
                    :x.Level == LogEventLevel.Error?ImGuiColors.DalamudRed
                    :x.Level == LogEventLevel.Warning?ImGuiColors.DalamudOrange
                    :x.Level == LogEventLevel.Information?ImGuiColors.DalamudWhite
                    :x.Level == LogEventLevel.Debug?ImGuiColors.DalamudGrey
                    :x.Level == LogEventLevel.Verbose?ImGuiColors.DalamudGrey2
                    :ImGuiColors.DalamudWhite2, x.Message);
            }
            ImGui.EndChild();
        }
    }
}
