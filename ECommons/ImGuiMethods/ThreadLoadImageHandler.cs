using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods.ImageLoading;
using ECommons.Logging;
using Lumina.Data.Files;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using TerraFX.Interop.WinRT;
using static Dalamud.Plugin.Services.ITextureProvider;
using static ECommons.GenericHelpers;

namespace ECommons.ImGuiMethods;
#nullable disable

public class ThreadLoadImageHandler
{
    internal static ConcurrentDictionary<string, ImageLoadingResult> CachedTextures = [];
    internal static ConcurrentDictionary<(GameIconLookup Lookup, int Width, int Height), ImageLoadingResult> CachedIcons = [];

    private static readonly List<Func<byte[], byte[]>> _conversionsToBitmap = [b => b,];
    private static volatile bool ThreadRunning = false;
    internal static HttpClient httpClient = null;

    /// <summary>
    /// Override error action if you wish. Will be executed on non-game main thread.
    /// </summary>
    public static Action<Exception?, string?>? ErrorAction = null;

    /// <summary>
    /// Clears and disposes all cached resources. You can use it to free up memory once you think textures that you have previously loaded won't be needed for a while or to trigger a complete reload.
    /// </summary>
    public static void ClearAll()
    {
        foreach(var x in CachedTextures)
        {
            Safe(() => { x.Value.TextureWrap?.Dispose(); });
        }
        Safe(CachedTextures.Clear);
        ClearIcons();
    }

    public static void ClearIcons()
    {
        foreach(var x in CachedIcons)
        {
            Safe(() => { x.Value.TextureWrap?.Dispose(); });
        }
        Safe(CachedIcons.Clear);
    }

    internal static void InvalidateIcon(uint iconId)
    {
        foreach(var x in CachedIcons)
        {
            if(x.Key.Lookup.IconId != iconId) continue;
            if(CachedIcons.TryRemove(x.Key, out var result)) Safe(() => { result.TextureWrap?.Dispose(); });
        }
    }

    /// <inheritdoc cref="TryGetIconTextureWrap(uint, bool, out IDalamudTextureWrap)" />
    public static bool TryGetIconTextureWrap(int icon, bool hq, out IDalamudTextureWrap textureWrap) => TryGetIconTextureWrap((uint)icon, hq, out textureWrap);

    /// <summary>
    /// Attempts to load game icon. <b>Do NOT cache <paramref name="textureWrap"/></b> and call this function every time before you want to work with it.
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="hq"></param>
    /// <param name="textureWrap"></param>
    /// <returns></returns>
    public static bool TryGetIconTextureWrap(uint icon, bool hq, out IDalamudTextureWrap textureWrap) => TryGetIconTextureWrap(new GameIconLookup(icon, hiRes: hq), out textureWrap);

    public static bool TryGetIconTextureWrap(GameIconLookup lookup, out IDalamudTextureWrap textureWrap) => TryGetSharedIcon(lookup, out textureWrap);

    public static bool TryGetResampledIcon(int icon, bool hq, int width, int height, out IDalamudTextureWrap textureWrap) => TryGetResampledIcon((uint)icon, hq, width, height, out textureWrap);

    public static bool TryGetResampledIcon(uint icon, bool hq, int width, int height, out IDalamudTextureWrap textureWrap) => TryGetResampledIcon(new GameIconLookup(icon, hiRes: hq), width, height, out textureWrap);

    public static bool TryGetResampledIcon(GameIconLookup lookup, int width, int height, out IDalamudTextureWrap textureWrap)
    {
        if(!TryResolveRequestedSize(ref width, ref height)) return TryGetSharedIcon(lookup, out textureWrap);

        var key = (lookup, width, height);
        var added = false;
        var result = CachedIcons.GetOrAdd(key, _ => { added = true; return new(); });
        if(added) BeginThreadIfNotRunning();
        textureWrap = result.TextureWrap;
        if(textureWrap != null) return true;
        return TryGetSharedIcon(lookup, out textureWrap);
    }

    private static bool TryGetSharedIcon(GameIconLookup lookup, out IDalamudTextureWrap textureWrap)
    {
        textureWrap = Svc.Texture.TryGetFromGameIcon(lookup, out var shared) ? shared.GetWrapOrDefault() : null;
        return textureWrap != null;
    }

    private static bool TryResolveRequestedSize(ref int width, ref int height)
    {
        if(GameIcons.Resize == null) return false;
        if(width <= 0) width = height;
        if(height <= 0) height = width;
        if(width <= 0 || height <= 0) return false;
        return width <= GameIcons.MaximumSize && height <= GameIcons.MaximumSize;
    }

    /// <summary>
    /// Attempts to load image from URL, game path or file on disk. <b>Do NOT cache <paramref name="textureWrap"/></b> and call this function every time before you want to work with it.
    /// </summary>
    /// <param name="url">URL, game path or file on disk</param>
    /// <param name="textureWrap"></param>
    /// <returns></returns>
    public static bool TryGetTextureWrap(string url, out IDalamudTextureWrap textureWrap)
    {
        ImageLoadingResult result;
        if(!CachedTextures.TryGetValue(url, out result))
        {
            result = new();
            CachedTextures[url] = result;
            PluginLog.Debug($"[ThreadLoadImageHandler] Requesting {url} for the first time");
            BeginThreadIfNotRunning();
        }
        textureWrap = result.Texture;
        return result.Texture != null;
    }

    internal static void BeginThreadIfNotRunning()
    {
        httpClient ??= new()
        {
            Timeout = TimeSpan.FromSeconds(10),
        };
        if(ThreadRunning) return;
        PluginLog.Verbose("Starting ThreadLoadImageHandler");
        ThreadRunning = true;
        new Thread(() =>
        {
            var idleTicks = 0;
            try
            {
                while(idleTicks < 100)
                {
                    try
                    {
                        {
                            if(CachedTextures.TryGetFirst(x => x.Value.IsCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.IsCompleted = true;
                                PluginLog.Verbose("Loading image " + keyValuePair.Key);
                                if(keyValuePair.Key.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || keyValuePair.Key.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                                {
                                    var result = httpClient.GetAsync(keyValuePair.Key).Result;
                                    result.EnsureSuccessStatusCode();
                                    var content = result.Content.ReadAsByteArrayAsync().Result;

                                    IDalamudTextureWrap texture = null;
                                    List<Exception> exceptions = [];
                                    foreach(var conversion in _conversionsToBitmap)
                                    {
                                        if(conversion == null) continue;

                                        try
                                        {
                                            texture = Svc.Texture.CreateFromImageAsync(conversion(content)).Result;
                                            if(texture != null) goto Success;
                                        }
                                        catch(Exception ex)
                                        {
                                            exceptions.Add(ex);
                                        }
                                    }
                                    if(ErrorAction != null)
                                    {
                                        ErrorAction(null, $"While loading {keyValuePair.Key} an exception occurred:");
                                        exceptions.Each(x => ErrorAction(x, null));
                                    }
                                    else
                                    {
                                        PluginLog.Error($"While loading {keyValuePair.Key} an exception occurred:");
                                        exceptions.Each(x => x.Log());
                                    }
                                Success:
                                    keyValuePair.Value.TextureWrap = texture;
                                }
                                else
                                {
                                    if(File.Exists(keyValuePair.Key))
                                    {
                                        keyValuePair.Value.ImmediateTexture = Svc.Texture.GetFromFile(keyValuePair.Key);
                                    }
                                    else
                                    {
                                        keyValuePair.Value.ImmediateTexture = Svc.Texture.GetFromGame(keyValuePair.Key);
                                    }
                                }
                            }
                        }
                        {
                            if(CachedIcons.TryGetFirst(x => x.Value.IsCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.IsCompleted = true;
                                var key = keyValuePair.Key;
                                PluginLog.Verbose($"Resampling icon {key.Lookup} to {key.Width}x{key.Height}");
                                var resampled = ResizeIcon(key.Lookup, key.Width, key.Height);
                                keyValuePair.Value.TextureWrap = resampled;
                                if(resampled != null && (!CachedIcons.TryGetValue(key, out var current) || !ReferenceEquals(current, keyValuePair.Value)))
                                {
                                    Safe(resampled.Dispose);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        if(ErrorAction != null)
                        {
                            ErrorAction(e, $"An error occurred while loading icon");
                        }
                        else
                        {
                            e.Log();
                        }
                    }
                    idleTicks++;
                    if(!CachedTextures.Any(x => x.Value.IsCompleted) && !CachedIcons.Any(x => x.Value.IsCompleted)) Thread.Sleep(100);
                }
            }
            catch(Exception e)
            {
                if(ErrorAction != null)
                {
                    ErrorAction(e, $"An error occurred while running ThreadLoadImageHandler");
                }
                else
                {
                    e.Log();
                }
            }
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

    private static IDalamudTextureWrap ResizeIcon(GameIconLookup lookup, int width, int height)
    {
        var resize = GameIcons.Resize;
        if(resize == null) return null;
        if(!TryGetIconTexFile(lookup, out var file) || file.Header.Width == 0 || file.Header.Height == 0)
        {
            PluginLog.Warning($"[ThreadLoadImageHandler] Could not find icon {lookup.IconId}");
            return null;
        }
        return resize(file, width, height);
    }

    private static bool TryGetIconTexFile(GameIconLookup lookup, out TexFile file)
    {
        file = null;
        if(!Svc.Texture.TryGetIconPath(lookup, out var path)) return false;

        var substituted = Svc.TextureSubstitution.GetSubstitutedPath(path);
        if(substituted != path && Path.IsPathRooted(substituted))
        {
            if(substituted.EndsWith(".tex", StringComparison.OrdinalIgnoreCase) && File.Exists(substituted))
                file = Svc.Data.GameData.GetFileFromDisk<TexFile>(substituted, path);
        }
        else
        {
            file = Svc.Data.GetFile<TexFile>(substituted);
        }

        file ??= Svc.Data.GetFile<TexFile>(path);
        return file != null;
    }
}