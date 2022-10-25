using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public static class GradientColor
    {
        public static Vector4 Get(Vector4 start, Vector4 end, int Miliseconds = 1000)
        {
            var delta = (end - start) / (int)Miliseconds;
            var time = Environment.TickCount % (Miliseconds * 2);
            if (Environment.TickCount % (Miliseconds * 2) < Miliseconds)
            {
                return start + delta * (float)(time / 2);
            }
            else
            {
                return end - delta * ((float)((time) / 2));
            }
        }
    }
}
