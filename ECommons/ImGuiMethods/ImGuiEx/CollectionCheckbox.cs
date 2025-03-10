using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        if(!values.Any())
        {
            var x = false;
            ImGui.BeginDisabled();
            ImGui.Checkbox(label, ref x);
            ImGui.EndDisabled();
            return false;
        }
        void RemoveAll()
        {
            foreach(var el in values)
            {
                collection.Remove(el);
            }
        }
        void AddAll()
        {
            foreach(var el in values)
            {
                collection.Remove(el);
                collection.Add(el);
            }
        }
        if(!inverted)
        {
            if(collection.ContainsAll(values))
            {
                var x = true;
                if(ImGui.Checkbox(label, ref x))
                {
                    Execute(RemoveAll, delayedOperation);
                    return true;
                }
            }
            else
            {
                var x = collection.ContainsAny(values);
                if(ImGuiEx.CheckboxBullet(label, ref x))
                {
                    Execute(AddAll, delayedOperation);
                    return true;
                }
            }
        }
        else
        {
            if(!collection.ContainsAny(values))
            {
                var x = true;
                if(ImGui.Checkbox(label, ref x))
                {
                    Execute(AddAll, delayedOperation);
                    return true;
                }
            }
            else
            {
                var x = !collection.ContainsAll(values);
                if(ImGuiEx.CheckboxBullet(label, ref x))
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
        if(inverted) x = !x;
        if(draw(ref x))
        {
            Execute(delegate
            {
                if(inverted) x = !x;
                if(x)
                {
                    collection.Add(value);
                }
                else
                {
                    while(collection.Contains(value))
                    {
                        if(!collection.Remove(value)) break;
                    }
                }
            }, delayedOperation);
            return true;
        }
        return false;
    }

    public static bool CollectionButtonCheckbox<T>(string name, T value, ICollection<T> collection, bool smallButton = false, bool inverted = false) => CollectionButtonCheckbox(name, value, collection, EzColor.Red, smallButton, inverted);
    public static bool CollectionButtonCheckbox<T>(string name, T value, ICollection<T> collection, Vector4 color, bool smallButton = false, bool inverted = false)
    {
        var col = collection.Contains(value);
        if(inverted) col = !col;
        var ret = false;
        if(col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if(smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if(col)
            {
                if(inverted)
                {
                    collection.Add(value);
                }
                else
                {
                    collection.Remove(value);
                }
            }
            else
            {
                if(inverted)
                {
                    collection.Remove(value);
                }
                else
                {
                    collection.Add(value);
                }
            }
            ret = true;
        }
        if(col) ImGui.PopStyleColor(3);
        return ret;
    }
}
