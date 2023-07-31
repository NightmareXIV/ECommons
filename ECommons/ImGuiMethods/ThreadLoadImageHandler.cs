﻿using ECommons.DalamudServices;
using ECommons.Logging;
using ImGuiScene;
using Svg;
using System;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using static ECommons.GenericHelpers;

namespace ECommons.ImGuiMethods;

public class ThreadLoadImageHandler
{
    internal static ConcurrentDictionary<string, ImageLoadingResult> CachedTextures = new();
    internal static ConcurrentDictionary<(uint ID, bool HQ), ImageLoadingResult> CachedIcons = new();

    static volatile bool ThreadRunning = false;
    internal static HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10),
    };

    internal static void Dispose()
    {
        foreach (var x in CachedTextures)
        {
            Safe(() => { x.Value.texture?.Dispose(); });
        }
        Safe(CachedTextures.Clear);
        foreach (var x in CachedIcons)
        {
            Safe(() => { x.Value.texture?.Dispose(); });
        }
        Safe(CachedIcons.Clear);
    }

    public static bool TryGetIconTextureWrap(uint icon, bool hq, out TextureWrap textureWrap)
    {
        ImageLoadingResult result;
        if (!CachedIcons.TryGetValue((icon, hq), out result))
        {
            result = new();
            CachedIcons[(icon, hq)] = result;
            BeginThreadIfNotRunning();
        }
        textureWrap = result.texture;
        return result.texture != null;
    }

    public static bool TryGetTextureWrap(string url, out TextureWrap textureWrap)
    {
        ImageLoadingResult result;
        if(!CachedTextures.TryGetValue(url, out result))
        {
            result = new();
            CachedTextures[url] = result;
            BeginThreadIfNotRunning();
        }
        textureWrap = result.texture;
        return result.texture != null;
    }

    internal static void BeginThreadIfNotRunning()
    {
        if (ThreadRunning) return;
        PluginLog.Information("Starting ThreadLoadImageHandler");
        ThreadRunning = true;
        new Thread(() =>
        {
            int idleTicks = 0;
            Safe(delegate
            {
                while(idleTicks < 100)
                {
                    Safe(delegate
                    {
                        {
                            if (CachedTextures.TryGetFirst(x => x.Value.isCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.isCompleted = true;
                                PluginLog.Information("Loading image " + keyValuePair.Key);
                                if (keyValuePair.Key.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || keyValuePair.Key.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                                {
                                    var result = httpClient.GetAsync(keyValuePair.Key).Result;
                                    result.EnsureSuccessStatusCode();
                                    var content = result.Content.ReadAsByteArrayAsync().Result;
                                    try
                                    {
                                        keyValuePair.Value.texture = Svc.PluginInterface.UiBuilder.LoadImage(content);
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            using var stream = new MemoryStream(content);
                                            using var outStream = new MemoryStream();
                                            var svgDocument = SvgDocument.Open<SvgDocument>(stream);
                                            var bitmap = svgDocument.Draw();
                                            bitmap.Save(outStream, ImageFormat.Png);
                                            content = outStream.ToArray();
                                            keyValuePair.Value.texture = Svc.PluginInterface.UiBuilder.LoadImage(content);
                                        }
                                        catch
                                        {
                                            throw ex;
                                            throw;
                                        }
                                    }
                                }
                                else
                                {
                                    if (File.Exists(keyValuePair.Key))
                                    {
                                        keyValuePair.Value.texture = Svc.PluginInterface.UiBuilder.LoadImage(keyValuePair.Key);
                                    }
                                    else
                                    {
                                        keyValuePair.Value.texture = Svc.Data.GetImGuiTexture(keyValuePair.Key);
                                    }
                                }
                            }
                        }
                        {
                            if (CachedIcons.TryGetFirst(x => x.Value.isCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.isCompleted = true;
                                PluginLog.Information($"Loading icon {keyValuePair.Key.ID}, hq={keyValuePair.Key.HQ}");
                                keyValuePair.Value.texture = Svc.Data.GetImGuiTextureIcon(keyValuePair.Key.HQ, keyValuePair.Key.ID);
                            }
                        }
                    });
                    idleTicks++;
                    Thread.Sleep(100);
                }
            });
            PluginLog.Information($"Stopping ThreadLoadImageHandler, ticks={idleTicks}");
            ThreadRunning = false;
        }).Start();
    }
}
