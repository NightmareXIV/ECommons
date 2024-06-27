using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    /// <summary>
    /// Checkbox that adds/removes a set of values from the collection. Displays check mark when element is present in a collection, and doesn't displays checkmark when element is not present.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="label"></param>
    /// <param name="values">A set of values to add/remove</param>
    /// <param name="collection">A collection that will be modified</param>
    /// <param name="inverted">Whether to invert checkbox. Not implemented.</param>
    /// <param name="delayedOperation">When set to true, will schedule the change in next framework update. Useful when you want to modify a collection while iterating over it.</param>
    /// <returns></returns>
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
        if (!inverted)
        {
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
        }
        else
        {
            if (!collection.ContainsAny(values))
            {
                var x = true;
                if (ImGui.Checkbox(label, ref x))
                {
                    Execute(AddAll, delayedOperation);
                    return true;
                }
            }
            else
            {
                var x = !collection.ContainsAll(values);
                if (ImGuiEx.CheckboxBullet(label, ref x))
                {
                    Execute(RemoveAll, delayedOperation);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Checkbox that adds/removes a value from the collection. Displays check mark when element is present in a collection, and doesn't displays checkmark when element is not present.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="label"></param>
    /// <param name="value">A value to add/remove</param>
    /// <param name="collection">A collection that will be modified</param>
    /// <param name="inverted">Whether to invert checkbox.</param>
    /// <param name="delayedOperation">When set to true, will schedule the change in next framework update. Useful when you want to modify a collection while iterating over it.</param>
    /// <returns></returns>
    public static bool CollectionCheckbox<T>(string label, T value, ICollection<T> collection, bool inverted = false, bool delayedOperation = false)
    {
        bool Draw(ref bool x) => ImGui.Checkbox(label, ref x);
        return CollectionCore(Draw, value, collection, inverted, delayedOperation);
    }

    public delegate bool CollectionCoreDelegate(ref bool contains);
    public static bool CollectionCore<T>(CollectionCoreDelegate draw, T value, ICollection<T> collection, bool inverted = false, bool delayedOperation = false)
    {
        var x = collection.Contains(value);
        if (inverted) x = !x;
        if (draw(ref x))
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
