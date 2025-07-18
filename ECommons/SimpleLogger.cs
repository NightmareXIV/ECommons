using ECommons.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommons;

public class SimpleLogger : IDisposable
{
    private BlockingCollection<string> logQueue = [];
    public SimpleLogger(string dir, string filename)
    {
        filename = filename.Split(Path.GetInvalidFileNameChars()).Join("_");
        new Thread((ThreadStart)delegate
        {
            try
            {
                using(StreamWriter stream = new(Path.Combine(dir, filename), true, Encoding.UTF8))
                {
                    stream.WriteLine($"Log begins at {DateTimeOffset.Now:s}");
                    try
                    {
                        while(!logQueue.IsCompleted)
                        {
                            var str = $"{logQueue.Take()}";
                            stream.WriteLine(str);
                        }
                    }
                    catch(InvalidOperationException e)
                    {
                        PluginLog.Information($"Not an error: {e.Message}\n{e.StackTrace ?? ""}");
                    }
                    stream.WriteLine($"Log ends at {DateTimeOffset.Now:s}");
                }
            }
            catch(InvalidOperationException e)
            {
                PluginLog.Debug($"Not an error: {e.Message}\n{e.StackTrace ?? ""}");
            }
            catch(Exception e)
            {
                PluginLog.Debug($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }).Start();
    }

    public void Dispose()
    {
        try
        {
            logQueue.CompleteAdding();
        }catch(Exception e)
        {
            e.LogDebug();
        }
    }

    public void Log(string s)
    {
        if(logQueue.IsAddingCompleted)
        {
            PluginLog.Debug($"Can not log, collection is marked as completed\n{s}");
            return;
        }
        s = $"{DateTimeOffset.Now:s} {s}";
        if(!logQueue.TryAdd(s))
        {
            Task.Run(delegate { try { logQueue.Add(s); } catch(Exception e) { e.Log(); } });
        }
    }

    public void LogError(string s)
    {
        Log($"Error: {s}");
    }
}
