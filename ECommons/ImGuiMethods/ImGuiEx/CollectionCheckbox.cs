using ECommons.Schedulers;
using ImGuiNET;
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

    public static bool CollectionCheckbox<T>(string label, IEnumerable<T> values, ICollection<T> collection, bool inverted = false, bool delayedOperation = false)
    {
        if (!values.Any()) throw new InvalidOperationException("values can not be empty");
        void RemoveAll()
        {
            foreach (var el in values)
            {
                collection.Remove(el);
            }
        }
        void AddAll()
        {
            foreach (var el in values)
            {
                collection.Remove(el);
                collection.Add(el);
            }
        }
        if (collection.ContainsAll(values))
        {
            var x = true;
            if (ImGui.Checkbox(label, ref x))
            {
                Execute(RemoveAll, delayedOperation);
                return true;
            }
        }
        else
        {
            var x = collection.ContainsAny(values);
            if (ImGuiEx.CheckboxBullet(label, ref x))
            {
                Execute(AddAll, delayedOperation);
                return true;
            }
        }
        return false;
    }

    public static bool CollectionCheckbox<T>(string label, T value, ICollection<T> collection, bool inverted = false, bool delayedOperation = false)
    {
        var x = collection.Contains(value);
        if (inverted) x = !x;
        if (ImGui.Checkbox(label, ref x))
        {
            Execute(delegate
            {
                if (inverted) x = !x;
                if (x)
                {
                    collection.Add(value);
                }
                else
                {
                    while (collection.Contains(value))
                    {
                        if (!collection.Remove(value)) break;
                    }
                }
            }, delayedOperation);
            return true;
        }
        return false;
    }
}
