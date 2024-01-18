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
    public static void RightFloat(Action drawAction) => RightFloat(GenericHelpers.GetCallStackID(), drawAction, out _);
    public static void RightFloat(Action drawAction, out float width) => RightFloat(GenericHelpers.GetCallStackID(), drawAction, out width);
    public static void RightFloat(string id, Action drawAction) => RightFloat(id, drawAction, out _);
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
