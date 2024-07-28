using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public static class GradientColor
{
    public static Vector4 Get(Vector4 start, Vector4 end, int Milliseconds = 1000)
    {
        var delta = (end - start) / (int)Milliseconds;
        var time = Environment.TickCount64 % (Milliseconds * 2);
        if(time < Milliseconds)
        {
            return start + delta * (float)(time % Milliseconds);
        }
        else
        {
            return end - delta * ((float)(time % Milliseconds));
        }
    }

    public static bool IsColorInRange(Vector4 testedColor, Vector4 start, Vector4 end, float tolerance = 0.1f)
    {
        return Test(testedColor.X, start.X, end.X)
            && Test(testedColor.Y, start.Y, end.Y)
            && Test(testedColor.Z, start.Z, end.Z)
            && Test(testedColor.W, start.W, end.W);
        bool Test(float tested, float a1, float a2)
        {
            var min = MathF.Min(a1, a2) - tolerance;
            var max = MathF.Max(a1, a2) + tolerance;
            return tested >= min && tested <= max;
        }
    }
}
