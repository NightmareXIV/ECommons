using ImGuiNET;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    /// <summary>
    /// <see cref="ImGui.Checkbox"/> that has bullet marker instead of normal check mark when enabled.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool CheckboxBullet(string label, ref bool value)
    {
        int flags = value ? 1 : 0;
        if (ImGui.CheckboxFlags(label, ref flags, int.MaxValue))
        {
            value = !value;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Inverted <see cref="ImGui.Checkbox"/>
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool CheckboxInverted(string label, ref bool value)
    {
        var inv = !value;
        if (ImGui.Checkbox(label, ref inv))
        {
            value = !inv;
            return true;
        }
        return false;
    }

    /// <summary>
    /// <see cref="ImGui.Checkbox"/> that accepts int as a bool. 0 is false, 1 is true.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Checkbox(string label, ref int value)
    {
        var b = value != 0;
        if (ImGui.Checkbox(label, ref b))
        {
            value = b ? 1 : 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tri-way <see cref="ImGui.Checkbox"/>. Null will be displayed as a bullet. Switching order: false -> null -> true.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Checkbox(string label, ref bool? value)
    {
        if(value != null)
        {
            var b = value.Value;
            if(ImGui.Checkbox(label, ref b))
            {
                if(b)
                {
                    value = null;
                }
                else
                {
                    value = false;
                }
                return true;
            }
        }
        else
        {
            var b = true;
            if(ImGuiEx.CheckboxBullet(label, ref b))
            {
                value = true;
                return true;
            }
        }
        return false;
    }
}
