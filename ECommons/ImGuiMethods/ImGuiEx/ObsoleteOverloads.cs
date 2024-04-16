using System;
using System.Collections.Generic;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    [Obsolete("Please switch to CollectionCheckbox")]
    public static bool HashSetCheckbox<T>(string label, T value, HashSet<T> collection) => CollectionCheckbox(label, value, collection);
    [Obsolete("Please switch to LineCentered")]
    public static void ImGuiLineCentered(string id, Action func) => LineCentered(id, func);
}
