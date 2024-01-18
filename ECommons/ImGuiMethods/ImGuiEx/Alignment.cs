using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
    public static void RightFloat(string id, Action drawAction, out float width)
    {
        if(RightFloatWidthCache.TryGetValue(id, out var value))
        {
            var cur = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X - value);
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
}
