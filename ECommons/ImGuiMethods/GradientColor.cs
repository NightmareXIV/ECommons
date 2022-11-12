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
            return start + delta * (float)(time % 1000);
        }
        else
        {
            return end - delta * ((float)(time % 1000));
        }
    }
}
