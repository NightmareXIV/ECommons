using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.ImGuiMethods;

public static partial class ImGuiEx
{
    extension(ImGui)
    {
        public static void PushID<T>(T g)
        {
            ImGui.PushID(g.ToString());
        }
    }
}
