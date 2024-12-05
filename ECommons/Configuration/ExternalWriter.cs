using ECommons.DalamudServices;
using ECommons.Logging;
using Lumina.Data.Parsing.Tex.Buffers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ECommons.Configuration;
public static class ExternalWriter
{
    private static BlockingCollection<FileSaveStruct>? FileSaveRequests = [];
    private static bool ThreadIsRunning = false;
    private volatile static bool Disposed = false;

    public static void PlaceWriteOrder(FileSaveStruct order)
    {
        if (FileSaveRequests == null)
        {
            PluginLog.Warning($"[FileWriterServer] FileSaveRequests is null, cannot place write order.");
            return;
        }

        if (!FileSaveRequests.TryAdd(order))
        {
            PluginLog.Warning($"[FileWriterServer] PlaceWriteOrder failed, trying on another tick");
            Svc.Framework.RunOnTick(() => PlaceWriteOrder(order));
        }
        else
        {
            if (!ThreadIsRunning)
            {
                ThreadIsRunning = true;
                BeginThread();
            }
        }
    }

    internal static void Dispose()
    {
        Disposed = true;
        FileSaveRequests?.CompleteAdding();
        FileSaveRequests?.Dispose();
    }

    static readonly string[] FileNames = ["ECommons.FileWriter.dll", "ECommons.FileWriter.deps.json", "ECommons.FileWriter.runtimeconfig.json"];

    private static void BeginThread()
    {
        new Thread(() =>
        {
            while(!Disposed)
            {
                try
                {
                    while(!FileSaveRequests!.IsCompleted)
                    {
                        var item = FileSaveRequests.Take();
                        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XIVLauncher", "runtime", "dotnet.exe");
                        foreach(var x in FileNames)
                        {
                            var sourcePath = Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName!, x);
                            var copyPath = Path.Combine(Svc.PluginInterface.ConfigDirectory.FullName, x);
                            if(!File.Exists(copyPath) || !FilesAreEqual(new(sourcePath), new(copyPath)))
                            {
                                PluginLog.Information($"Copying {sourcePath}->{copyPath}");
                                File.Copy(sourcePath, copyPath, true);
                            }
                            else
                            {
                                PluginLog.Information($"Files equal: {sourcePath}={copyPath}");
                            }
                        }
                        var dllPath = Path.Combine(Svc.PluginInterface.ConfigDirectory.FullName, "ECommons.FileWriter.dll");
                        var pipeClient = new Process();

                        pipeClient.StartInfo.FileName = path;

                        using(var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                        {
                            PluginLog.Debug($"[FileWriterServer] Current TransmissionMode: {pipeServer.TransmissionMode}.");

                            pipeClient.StartInfo.ArgumentList.Add(dllPath);
                            pipeClient.StartInfo.ArgumentList.Add(pipeServer.GetClientHandleAsString());
                            pipeClient.StartInfo.UseShellExecute = false;
                            pipeClient.StartInfo.CreateNoWindow = true;
                            pipeClient.StartInfo.RedirectStandardOutput = true;
                            pipeClient.StartInfo.RedirectStandardError = true;
                            pipeClient.OutputDataReceived += PipeClient_OutputDataReceived;
                            pipeClient.ErrorDataReceived += PipeClient_ErrorDataReceived;
                            pipeClient.Start();
                            pipeClient.BeginErrorReadLine();
                            pipeClient.BeginOutputReadLine();

                            pipeServer.DisposeLocalCopyOfClientHandle();

                            using var sw = new StreamWriter(pipeServer);
                            do
                            {
                                sw.AutoFlush = true;
                                sw.WriteLine(item.Serialize());
                                pipeServer.WaitForPipeDrain();
                                item = FileSaveRequests.Take();
                            }
                            while(!FileSaveRequests!.IsCompleted);

                            pipeClient.WaitForExit();
                            pipeClient.Close();
                            PluginLog.Information("[FileWriterServer] Client quit. Server terminating.");
                        }
                    }
                }
                catch(Exception e)
                {
                    e.Log();
                }
            }
        }).Start();
    }
    
    static bool FilesAreEqual(FileInfo first, FileInfo second)
    {
        int BYTES_TO_READ = sizeof(Int64);
        if(first.Length != second.Length)
            return false;

        if(string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            return true;

        int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

        using(FileStream fs1 = first.OpenRead())
        using(FileStream fs2 = second.OpenRead())
        {
            byte[] one = new byte[BYTES_TO_READ];
            byte[] two = new byte[BYTES_TO_READ];

            for(int i = 0; i < iterations; i++)
            {
                fs1.Read(one, 0, BYTES_TO_READ);
                fs2.Read(two, 0, BYTES_TO_READ);

                if(BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }
        }

        return true;
    }

    private static void PipeClient_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if(e.Data != null) PluginLog.Error(e.Data);
    }

    private static void PipeClient_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if(e.Data != null) PluginLog.Information(e.Data);
    }

    public class FileSaveStruct
    {
        [Obfuscation] public string? Name { get; set; }
        [Obfuscation] public byte[]? Data { get; set; }
        [Obfuscation] public byte[]? NameHash { get; set; }
        [Obfuscation]public byte[]? DataHash { get; set; }

        public FileSaveStruct(string name, string data)
        {
            Name = name;
            NameHash = SHA1.HashData(Encoding.UTF8.GetBytes(name));
            var dataBytes = Encoding.UTF8.GetBytes(data);
            Data = dataBytes;
            DataHash = SHA1.HashData(dataBytes);
        }

        public FileSaveStruct(string name, byte[] dataBytes)
        {
            Name = name;
            NameHash = SHA1.HashData(Encoding.UTF8.GetBytes(name));
            Data = dataBytes;
            DataHash = SHA1.HashData(dataBytes);
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
