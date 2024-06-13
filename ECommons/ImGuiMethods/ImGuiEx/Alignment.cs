using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    static Dictionary<string, float> RightFloatWidthCache = [];
    /// <summary>
    /// Draws a line on the right side of the window. Without specifying ID explicitly, you may only draw one line per unique action.
    /// </summary>
    /// <param name="drawAction">Line draw action. You can only use one line in one action.</param>
    public static void RightFloat(Action drawAction) => RightFloat(GenericHelpers.GetCallStackID(), drawAction, out _);
    /// <summary>
    /// Draws a line on the right side of the window. Without specifying ID explicitly, you may only draw one line per unique action.
    /// </summary>
    /// <param name="drawAction">Line draw action. You can only use one line in one action.</param>
    /// <param name="width">Calculated width of your line.</param>
    public static void RightFloat(Action drawAction, out float width) => RightFloat(GenericHelpers.GetCallStackID(), drawAction, out width);
    /// <summary>
    /// Draws a line on the right side of the window.
    /// </summary>
    /// <param name="id">Unique ID of a line.</param>
    /// <param name="drawAction">Line draw action. You can only use one line in one action.</param>
    public static void RightFloat(string id, Action drawAction) => RightFloat(id, drawAction, out _);
    /// <summary>
    /// Draws a line on the right side of the window.
    /// </summary>
    /// <param name="id">Unique ID of a line.</param>
    /// <param name="drawAction">Line draw action. You can only use one line in one action.</param>
    /// <param name="width">Calculated width of your line.</param>
    /// <param name="contentRegionAvailX">Override content region avail if needed</param>
    public static void RightFloat(string id, Action drawAction, out float width, float? contentRegionAvailX = null)
    {
        if(RightFloatWidthCache.TryGetValue(id, out var value))
        {
            contentRegionAvailX ??= ImGui.GetContentRegionAvail().X;
            var cur = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(contentRegionAvailX.Value - value);
            DrawAndStore();
            ImGui.SetCursorPos(cur);
        }
        else
        {
            DrawAndStore();
        }
        width = RightFloatWidthCache[id];
        void DrawAndStore()
        {
            var pos1 = ImGui.GetCursorPosX();
            drawAction();
            ImGui.SameLine();
            var pos2 = ImGui.GetCursorPosX();
            RightFloatWidthCache[id] = pos2 - pos1;
        }
    }

    static readonly Dictionary<string, float> CenteredLineWidths = new();
    public static void LineCentered(Action func) => LineCentered(GenericHelpers.GetCallStackID(), func);
    public static void LineCentered(string id, Action func)
    {
        if (CenteredLineWidths.TryGetValue(id, out var dims))
        {
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2 - dims / 2);
        }
        var oldCur = ImGui.GetCursorPosX();
        func();
        ImGui.SameLine(0, 0);
        CenteredLineWidths[id] = ImGui.GetCursorPosX() - oldCur;
        ImGui.Dummy(Vector2.Zero);
    }



    static Dictionary<string, float> InputWithRightButtonsAreaValues = new();
    public static void InputWithRightButtonsArea(Action inputAction, Action rightAction) => InputWithRightButtonsArea(GenericHelpers.GetCallStackID(), inputAction, rightAction);
    /// <summary>
    /// Convenient way to display stretched input with button or other elements on it's right side.
    /// </summary>
    /// <param name="id">Unique ID</param>
    /// <param name="inputAction">A single element that accepts transformation by ImGui.SetNextItemWidth method</param>
    /// <param name="rightAction">A line of elements on the right side. Can contain multiple elements but only one line.</param>
    public static void InputWithRightButtonsArea(string id, Action inputAction, Action rightAction)
    {
        if (InputWithRightButtonsAreaValues.ContainsKey(id))
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - InputWithRightButtonsAreaValues[id]);
        }
        inputAction();
        ImGui.SameLine();
        var cur1 = ImGui.GetCursorPosX();
        rightAction();
        ImGui.SameLine(0, 0);
        InputWithRightButtonsAreaValues[id] = ImGui.GetCursorPosX() - cur1 + ImGui.GetStyle().ItemSpacing.X;
        ImGui.Dummy(Vector2.Zero);
    }
}
