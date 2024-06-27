using ECommons.Schedulers;
using System;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    static void Execute(Action a, bool isDelayed)
    {
        if (isDelayed)
        {
            new TickScheduler(a);
        }
        else
        {
            a();
        }
    }
}
