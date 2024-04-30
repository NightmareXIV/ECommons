using Dalamud.Interface.Colors;
using ECommons.CircularBuffers;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using ImGuiNET;
using Serilog.Events;
using System;
using System.Linq;
using System.Numerics;

namespace ECommons.Logging;
#nullable disable

public class InternalLog
{
    public static readonly CircularBuffer<InternalLogMessage> Messages = new(1000);

    public static (string, Action, Vector4, bool) ImGuiTab(bool draw = true) => (draw ? "Log" : null, PrintImgui, ImGuiColors.DalamudGrey, false);

    public static void Information(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Information));
    }
    public static void Error(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Error));
    }
    public static void Fatal(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Fatal));
    }
    public static void Debug(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Debug));
    }
    public static void Verbose(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Verbose));
    }
    public static void Warning(string s)
    {
        Messages.PushBack(new(s, LogEventLevel.Warning));
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

    [Flags]
    public enum FilterType : long
    {
        Verbose = 0b1,
        Debug = 0b10,
        Info = 0b100,
        Warning = 0b1000,
        Error = 0b10000,
        Fatal = 0b100000,
        Default = ~0
    }

    static string Search = "";
    static bool Autoscroll = true;
    static LogEventLevel SelectedLevel = LogEventLevel.Verbose;
    static FilterType Filter = FilterType.Default;
    public static void PrintImgui()
    {
        ImGui.Checkbox("##Autoscroll", ref Autoscroll);
        ImGuiEx.Tooltip("Autoscroll");
        ImGui.SameLine();
        if (ImGui.Button("Copy all"))
        {
#pragma warning disable
            GenericHelpers.Copy(Messages.Where(x => x.Level >= SelectedLevel).Select(x => $"[{x.Level}@{x.Time}] {x.Message}").Join("\n"));
#pragma warning restore
        }
        ImGui.SameLine();
        if (ImGui.Button("Clear"))
        {
            Messages.Clear();
        }
        ImGui.SameLine();
        ImGuiEx.SetNextItemFullWidth(-30);
        ImGui.InputTextWithHint("##Filter", "Filter...", ref Search, 100);
        ImGui.SameLine();
        if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.Filter, "##LogFilter")) ImGui.OpenPopup("filter_window");
        ImGuiEx.Tooltip("Log Filter");

        if (ImGui.BeginPopup("filter_window", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.Popup))
        {
            DrawFilterPopup();
            ImGui.EndPopup();
        }

        ImGui.BeginChild($"Plugin_log{DalamudReflector.GetPluginName()}");
        foreach (var x in Messages)
        {
            if (!ShouldDisplayLog(x.Level)) continue;
            if (Search == String.Empty || x.Level.ToString().EqualsIgnoreCase(Search) || x.Message.Contains(Search, StringComparison.OrdinalIgnoreCase))
                ImGuiEx.TextWrappedCopy(x.Level == LogEventLevel.Fatal ? ImGuiColors.DPSRed
                    : x.Level == LogEventLevel.Error ? ImGuiColors.DalamudRed
                    : x.Level == LogEventLevel.Warning ? ImGuiColors.DalamudOrange
                    : x.Level == LogEventLevel.Information ? ImGuiColors.DalamudWhite
                    : x.Level == LogEventLevel.Debug ? ImGuiColors.DalamudGrey
                    : x.Level == LogEventLevel.Verbose ? ImGuiColors.DalamudGrey2
                    : ImGuiColors.DalamudWhite2, $"> [{x.Time}] {x.Message}");
        }
        if (Autoscroll)
        {
            ImGui.SetScrollHereY();
        }
        ImGui.EndChild();
    }

    private static void DrawFilterPopup()
    {
        void FlagCheckbox(string label, FilterType flag)
        {
            var b = Filter.HasFlag(flag);
            if (!ImGui.Checkbox(label, ref b))
                return;
            if (b)
                Filter |= flag;
            else
                Filter &= ~flag;
        }
        FlagCheckbox("Verbose", FilterType.Verbose);
        FlagCheckbox("Debug", FilterType.Debug);
        FlagCheckbox("Info", FilterType.Info);
        FlagCheckbox("Warning", FilterType.Warning);
        FlagCheckbox("Error", FilterType.Error);
        FlagCheckbox("Fatal", FilterType.Fatal);
    }

    private static bool ShouldDisplayLog(LogEventLevel level)
    {
        switch (level)
        {
            case LogEventLevel.Verbose:
                if (Filter.HasFlag(FilterType.Verbose)) return true;
                break;
            case LogEventLevel.Debug:
                if (Filter.HasFlag(FilterType.Debug)) return true;
                break;
            case LogEventLevel.Information:
                if (Filter.HasFlag(FilterType.Info)) return true;
                break;
            case LogEventLevel.Warning:
                if (Filter.HasFlag(FilterType.Warning)) return true;
                break;
            case LogEventLevel.Error:
                if (Filter.HasFlag(FilterType.Error)) return true;
                break;
            case LogEventLevel.Fatal:
                if (Filter.HasFlag(FilterType.Fatal)) return true;
                break;
            default:
                break;
        }
        return false;
    }
}
