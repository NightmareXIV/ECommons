using ECommons.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
