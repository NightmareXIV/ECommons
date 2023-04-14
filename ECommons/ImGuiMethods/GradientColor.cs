using ECommons.MathHelpers;
using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public static class GradientColor
{
    public static Vector4 Get(Vector4 start, Vector4 end, int Miliseconds = 1000)
    {
        var delta = (end - start) / (int)Miliseconds;
        var time = Environment.TickCount64 % (Miliseconds * 2);
        if (time < Miliseconds)
        {
            return start + delta * (float)(time % Miliseconds);
        }
        else
        {
            return end - delta * ((float)(time % Miliseconds));
        }
    }
}
