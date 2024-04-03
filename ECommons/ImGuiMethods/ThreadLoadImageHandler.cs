﻿using Dalamud.Interface.Internal;
using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using static Dalamud.Plugin.Services.ITextureProvider;
using static ECommons.GenericHelpers;

namespace ECommons.ImGuiMethods;
#nullable disable

public class ThreadLoadImageHandler
{
    internal static ConcurrentDictionary<string, ImageLoadingResult> CachedTextures = new();
    internal static ConcurrentDictionary<(uint ID, IconFlags HQ), ImageLoadingResult> CachedIcons = new();

    private static readonly List<Func<byte[], byte[]>> _conversionsToBitmap = new() { b => b, };

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

    public static bool TryGetIconTextureWrap(int icon, bool hq, out IDalamudTextureWrap textureWrap) => TryGetIconTextureWrap((uint)icon, hq, out textureWrap);
    public static bool TryGetIconTextureWrap(int icon, IconFlags hq, out IDalamudTextureWrap textureWrap) => TryGetIconTextureWrap((uint)icon, hq, out textureWrap);

    public static bool TryGetIconTextureWrap(uint icon, bool hq, out IDalamudTextureWrap textureWrap)
    {
        return TryGetIconTextureWrap(icon, hq ? IconFlags.HiRes : IconFlags.None, out textureWrap);
    }
    public static bool TryGetIconTextureWrap(uint icon, IconFlags hq, out IDalamudTextureWrap textureWrap)
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

    public static bool TryGetTextureWrap(string url, out IDalamudTextureWrap textureWrap)
    {
        ImageLoadingResult result;
        if (!CachedTextures.TryGetValue(url, out result))
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
        PluginLog.Verbose("Starting ThreadLoadImageHandler");
        ThreadRunning = true;
        new Thread(() =>
        {
            int idleTicks = 0;
            Safe(delegate
            {
                while (idleTicks < 100)
                {
                    Safe(delegate
                    {
                        {
                            if (CachedTextures.TryGetFirst(x => x.Value.isCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.isCompleted = true;
                                PluginLog.Verbose("Loading image " + keyValuePair.Key);
                                if (keyValuePair.Key.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || keyValuePair.Key.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                                {
                                    var result = httpClient.GetAsync(keyValuePair.Key).Result;
                                    result.EnsureSuccessStatusCode();
                                    var content = result.Content.ReadAsByteArrayAsync().Result;

                                    IDalamudTextureWrap texture = null;
                                    foreach (var conversion in _conversionsToBitmap)
                                    {
                                        if (conversion == null) continue;

                                        try
                                        {
                                            texture = Svc.PluginInterface.UiBuilder.LoadImage(conversion(content));
                                            if (texture != null) break;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Log();
                                        }
                                    }
                                    keyValuePair.Value.texture = texture;
                                }
                                else
                                {
                                    if (File.Exists(keyValuePair.Key))
                                    {
                                        keyValuePair.Value.texture = Svc.PluginInterface.UiBuilder.LoadImage(keyValuePair.Key);
                                    }
                                    else
                                    {
                                        keyValuePair.Value.texture = Svc.Texture.GetTextureFromGame(keyValuePair.Key);
                                    }
                                }
                            }
                        }
                        {
                            if (CachedIcons.TryGetFirst(x => x.Value.isCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.isCompleted = true;
                                PluginLog.Verbose($"Loading icon {keyValuePair.Key.ID}, hq={keyValuePair.Key.HQ}");
                                keyValuePair.Value.texture = Svc.Texture.GetIcon(keyValuePair.Key.ID, keyValuePair.Key.HQ);
                            }
                        }
                    });
                    idleTicks++;
                    if(!CachedTextures.Any(x => x.Value.isCompleted) && !CachedIcons.Any(x => x.Value.isCompleted)) Thread.Sleep(100);
                }
            });
            PluginLog.Verbose($"Stopping ThreadLoadImageHandler, ticks={idleTicks}");
            ThreadRunning = false;
        }).Start();
    }

    public static void AddConversionToBitmap(Func<byte[], byte[]> conversion)
    {
        _conversionsToBitmap.Add(conversion);
    }

    public static void RemoveConversionToBitmap(Func<byte[], byte[]> conversion)
    {
        _conversionsToBitmap.Remove(conversion);
    }
}