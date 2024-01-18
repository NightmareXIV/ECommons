using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    [Obsolete("Please switch to CollectionCheckbox")]
    public static bool HashSetCheckbox<T>(string label, T value, HashSet<T> collection) => CollectionCheckbox(label, value, collection);
}
