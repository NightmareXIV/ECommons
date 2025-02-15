using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ECommons.Interop;
#nullable disable

public class OpenFileDialog
{
    private static SemaphoreSlim SelectorSemaphore = new(1, 1);
    public static bool IsSelecting()
    {
        return SelectorSemaphore.CurrentCount == 0;
    }

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    public static void SelectFile(Action<OpenFileName> successCallback, Action cancelCallback = null, string initialDir = null, string title = "Select a file...", IEnumerable<(string Description, IEnumerable<string> Extensions)> fileTypes = null) => SelectFile(new(), successCallback, cancelCallback, initialDir, title, fileTypes);

    public static void SelectFile(OpenFileName ofn, Action<OpenFileName> successCallback, Action cancelCallback = null, string initialDir = null, string title = "Select a file...", IEnumerable<(string Description, IEnumerable<string> Extensions)> fileTypes = null)
    {
        if(SelectorSemaphore.Wait(0))
        {
            new Thread((ThreadStart)delegate
            {
                PluginLog.Debug("Starting file selection");
                try
                {

                    if(WindowFunctions.TryFindGameWindow(out var hwnd))
                    {
                        PluginLog.Information($"With owner: {hwnd:X16}");
                        ofn.dlgOwner = hwnd;
                    }

                    ofn.structSize = Marshal.SizeOf(ofn);

                    List<string> FileTypes = [];
                    if(fileTypes != null)
                    {
                        foreach(var x in fileTypes)
                        {
                            FileTypes.Add($"{x.Description}\0{x.Extensions.Select(x => $"*.{x}").Join(";")}");
                        }
                        FileTypes.Add("\0All files\0*\0");
                    }

                    ofn.filter = FileTypes.Join("\0");

                    ofn.file = new String(new char[1024]);
                    ofn.maxFile = ofn.file.Length;

                    ofn.fileTitle = new String(new char[256]);
                    ofn.maxFileTitle = ofn.fileTitle.Length;

                    ofn.initialDir = initialDir;
                    ofn.title = title;

                    PluginLog.Debug("Preparing to call winapi");
                    if(GetOpenFileName(ofn))
                    {
                        successCallback?.Invoke(ofn);
                    }
                    else
                    {
                        cancelCallback?.Invoke();
                    }
                    PluginLog.Debug("Dialog closed");
                }
                catch(Exception e)
                {
                    PluginLog.Error(e.Message + "\n" + e.StackTrace ?? "");
                    new TickScheduler(delegate
                    {
                        Notify.Error($"Error: {e.Message}");
                    });
                }
                SelectorSemaphore.Release();
                PluginLog.Debug("Ending file selection");
            }).Start();
        }
        else
        {
            Notify.Error("Failed to open file dialog");
        }
    }
}
