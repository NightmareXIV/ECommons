using ECommons.MathHelpers;
using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public static class GradientColor
{
    public static Vector4 Get(Vector4 start, Vector4 end, int Miliseconds = 1000)
    {
        var delta = (end - start) / (int)Miliseconds;
        var time = Environment.TickCount % (Miliseconds * 2);
        if (time < Miliseconds)
        {
            return start + delta * (float)(time % Miliseconds);
        }
        else
        {
            return end - delta * ((float)(time % Miliseconds));
        }
    }

    public static Vector4Double GetPrecise(Vector4Double start, Vector4Double end, int Miliseconds = 1000)
    {
        var delta = (end - start) / (int)Miliseconds;
        var time = Environment.TickCount % (Miliseconds * 2);
        if (time < Miliseconds)
        {
            return start + delta * (double)(time % Miliseconds);
        }
        else
        {
            return end - delta * ((double)(time % Miliseconds));
        }
    }
}
