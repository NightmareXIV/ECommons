
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
#if (DEBUGFORMS || RELEASEFORMS)
using System.Windows.Forms;
#endif

namespace ECommons;
public static partial class GenericHelpers
{
    /// <summary>
    /// Copies text into user's clipboard using WinForms. Does not throws exceptions.
    /// </summary>
    /// <param name="text">Text to copy</param>
    /// <param name="silent">Whether to display success/failure popup</param>
    /// <returns>Whether operation succeeded</returns>
#if !(DEBUGFORMS || RELEASEFORMS)
    [Obsolete("You have selected not to use Windows Forms; copying will be done via ImGui. This has been known to cause serious issues in past. If you are working with clipboard a lot, consider enabling Windows Forms.")]
#endif
    public static bool Copy(string text, bool silent = false)
    {
        try
        {
            if (text.IsNullOrEmpty())
            {
#if (DEBUGFORMS || RELEASEFORMS)
                Clipboard.Clear();
#else
                ImGui.SetClipboardText("");
#endif
                if (!silent) Notify.Success("Clipboard cleared");
            }
            else
            {
#if (DEBUGFORMS || RELEASEFORMS)
                Clipboard.SetText(text);
#else
                ImGui.SetClipboardText(text);
#endif
                if (!silent) Notify.Success("Text copied to clipboard");
            }
            return true;
        }
        catch (Exception e)
        {
            if (!silent)
            {
                Notify.Error($"Error copying to clipboard:\n{e.Message}\nPlease try again");
            }
            PluginLog.Warning($"Error copying to clipboard:");
            e.LogWarning();
            return false;
        }
    }

    /// <summary>
    /// Reads text from user's clipboard
    /// </summary>
    /// <param name="silent">Whether to display popup when error occurs.</param>
    /// <returns>Contents of the clipboard; null if clipboard couldn't be read.</returns>
#if !(DEBUGFORMS || RELEASEFORMS)
    [Obsolete("You have selected not to use Windows Forms; pasting will be done via ImGui. This has been known to cause serious issues in past. If you are working with clipboard a lot, consider enabling Windows Forms.")]
#endif
    public static string? Paste(bool silent = false)
    {
        try
        {
#if (DEBUGFORMS || RELEASEFORMS)
            return Clipboard.GetText();
#else
            return ImGui.GetClipboardText();
#endif
        }
        catch (Exception e)
        {
            if (!silent)
            {
                Notify.Error($"Error pasting from clipboard:\n{e.Message}\nPlease try again");
            }
            PluginLog.Warning($"Error pasting from clipboard:");
            e.LogWarning();
            return null;
        }
    }

#if (DEBUGFORMS || RELEASEFORMS)

    /// <summary>
    /// Checks if a key is pressed via winapi.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Whether the key is currently pressed</returns>
    public static bool IsKeyPressed(Keys key) => IsKeyPressed((int)key);


    public static bool IsKeyPressed(IEnumerable<Keys> keys)
    {
        foreach (var x in keys)
        {
            if (IsKeyPressed(x)) return true;
        }
        return false;
    }
#endif
}