using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public class TimedMiddleOverlayWindow : MiddleOverlayWindow
{
    private long destroyAt;
    public TimedMiddleOverlayWindow(string name, long destroyAfterMS, Action draw, int? topOffset = null, Vector4? bgCol = null) : base(name, draw, topOffset, bgCol)
    {
        destroyAt = Environment.TickCount64 + destroyAfterMS;
    }

    public override void Update()
    {
        if(Environment.TickCount64 > destroyAt)
        {
            Dispose();
        }
    }
}
