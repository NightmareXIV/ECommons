using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.WindowsFormsReflector;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace ECommons;
public static partial class GenericHelpers
{
    /// <summary>
    /// Copies text into user's clipboard using WinForms. Does not throws exceptions.
    /// </summary>
    /// <param name="text">Text to copy</param>
    /// <param name="silent">Whether to display success/failure popup</param>
    /// <returns>Whether operation succeeded</returns>
    public static bool Copy(string text, bool silent = false)
    {
        try
        {
            if(text.IsNullOrEmpty())
            {
                Winforms.Clipboard.Clear();
                if(!silent) Notify.Success("Clipboard cleared");
            }
            else
            {
                Winforms.Clipboard.SetText(text);
                if(!silent) Notify.Success("Text copied to clipboard");
            }
            return true;
        }
        catch(Exception e)
        {
            if(!silent)
            {
                Notify.Error($"Error copying to clipboard:\n{e.Message}\nPlease try again");
            }
            PluginLog.Warning($"Error copying to clipboard:");
            e.LogWarning();
            return false;
        }
    }

    /// <summary>
    /// Reads text from user's clipboard using Windows Forms
    /// </summary>
    /// <param name="silent">Whether to display popup when error occurs.</param>
    /// <returns>Contents of the clipboard; null if clipboard couldn't be read.</returns>
    /// <remarks>Be sure to run on the framework/draw thread if using ImGui to avoid potential crashes.</remarks>
    public static string? Paste(bool silent = false)
    {
        try
        {
            return Winforms.Clipboard.GetText();
        }
        catch(Exception e)
        {
            if(!silent)
            {
                Notify.Error($"Error pasting from clipboard:\n{e.Message}\nPlease try again");
            }
            PluginLog.Warning($"Error pasting from clipboard:");
            e.LogWarning();
            return null;
        }
    }

    /// <summary>
    /// Checks if a key is pressed via winapi.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Whether the key is currently pressed</returns>
    public static bool IsKeyPressed(Keys key) => IsKeyPressed((int)key);


    public static bool IsKeyPressed(IEnumerable<Keys> keys)
    {
        foreach(var x in keys)
        {
            if(IsKeyPressed(x)) return true;
        }
        return false;
    }
}