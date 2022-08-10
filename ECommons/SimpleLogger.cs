using Dalamud.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommons
{
    public class SimpleLogger : IDisposable
    {
        public static Action<string> OnLogError = null;
        public static Action<string> OnDuoLogMessage = null;
        BlockingCollection<string> logQueue = new();
        public SimpleLogger(string dir, string filename)
        {
            new Thread((ThreadStart)delegate 
            {
                try
                {
                    using (StreamWriter stream = new(Path.Combine(dir, filename), true, Encoding.UTF8))
                    {
                        stream.WriteLine($"Log begins at {DateTimeOffset.Now:s}");
                        while (!logQueue.IsCompleted)
                        {
                            var str = $"{logQueue.Take()}";
                            stream.WriteLine(str);
                        }
                        stream.WriteLine($"Log ends at {DateTimeOffset.Now:s}");
                    }
                }
                catch(InvalidOperationException e)
                {
                    PluginLog.Information($"Not an error: {e.Message}\n{e.StackTrace ?? ""}");
                }
                catch (Exception e)
                {
                    PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
                }
            }).Start();
        }

        public void Dispose()
        {
            logQueue.CompleteAdding();
        }

        public void Log(string s)
        {
            if (logQueue.IsAddingCompleted)
            {
                PluginLog.Error($"Can not log, collection is marked as completed");
            }
            s = $"{DateTimeOffset.Now:s} {s}";
            if (!logQueue.TryAdd(s))
            {
                Task.Run(delegate { logQueue.Add(s); });
            }
        }

        public void LogError(string s)
        {
            Log($"Error: {s}");
        }
    }
}
