using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;
using ECommons.Logging;
using Lumina.Data.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public static class GameIcons
{
    private static readonly ConcurrentDictionary<CacheKey, IDalamudTextureWrap?> Cache = [];

    public static IResampler Resampler { get; set; } = KnownResamplers.Lanczos3;
    public static int MaximumSize { get; set; } = 512;

    public static bool DrawInline(uint iconId, bool sameLine = true) => DrawInline(new GameIconLookup(iconId), sameLine);

    public static bool DrawInline(GameIconLookup lookup, bool sameLine = true)
    {
        var size = MathF.Round(ImGui.GetTextLineHeightWithSpacing());
        if(!Draw(lookup, new Vector2(size))) return false;
        if(sameLine) ImGui.SameLine();
        return true;
    }

    public static void DrawInlineOrIcon(uint? iconId, FontAwesomeIcon fallback, Vector4? fallbackColor = null)
    {
        if(iconId is { } id && DrawInline(id)) return;
        ImGuiEx.Icon(fallbackColor ?? ImGui.GetStyle().Colors[(int)ImGuiCol.Text], fallback);
    }

    public static bool Draw(uint iconId, float size) => Draw(new GameIconLookup(iconId), new Vector2(size));
    public static bool Draw(uint iconId, Vector2 size) => Draw(new GameIconLookup(iconId), size);

    public static bool Draw(GameIconLookup lookup, Vector2 size)
    {
        var width = (int)MathF.Round(size.X);
        var height = (int)MathF.Round(size.Y);
        if(Get(lookup, width, height) is not { } texture) return false;
        var position = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(new Vector2(MathF.Round(position.X), MathF.Round(position.Y)));
        ImGui.Image(texture.Handle, new Vector2(width, height));
        return true;
    }

    public static bool TryGetScaledIcon(uint iconId, int size, out IDalamudTextureWrap texture) => TryGetScaledIcon(new GameIconLookup(iconId), size, size, out texture);

    public static bool TryGetScaledIcon(GameIconLookup lookup, int width, int height, out IDalamudTextureWrap texture)
    {
        texture = Get(lookup, width, height);
        return texture != null;
    }

    public static void Invalidate(uint iconId)
    {
        foreach(var x in Cache)
        {
            if(x.Key.IconId != iconId) continue;
            if(Cache.TryRemove(x.Key, out var texture)) GenericHelpers.Safe(() => texture?.Dispose());
        }
    }

    public static void ClearAll()
    {
        foreach(var x in Cache)
        {
            GenericHelpers.Safe(() => x.Value?.Dispose());
        }
        GenericHelpers.Safe(Cache.Clear);
    }

    private static IDalamudTextureWrap? Get(GameIconLookup lookup, int width, int height)
    {
        if(width <= 0 || height <= 0 || width > MaximumSize || height > MaximumSize) return null;
        var key = new CacheKey(lookup.IconId, lookup.ItemHq, lookup.HiRes, lookup.Language, width, height);
        if(Cache.TryGetValue(key, out var cached)) return cached;

        IDalamudTextureWrap? texture = null;
        try
        {
            if(TryGetTexFile(lookup, out var file) && file.Header.Width > 0 && file.Header.Height > 0)
                texture = Resample(file, lookup.IconId, width, height);
            else
                PluginLog.Warning($"[GameIcons] Could not find icon {lookup.IconId}");
        }
        catch(Exception e)
        {
            PluginLog.Warning($"[GameIcons] Could not resample icon {lookup.IconId} to {width}x{height}:\n{e}");
        }

        Cache[key] = texture;
        return texture;
    }

    private static IDalamudTextureWrap Resample(TexFile file, uint iconId, int width, int height)
    {
        //these are bgra, not rgba
        using var image = Image.LoadPixelData<Bgra32>(file.ImageData, file.Header.Width, file.Header.Height);
        image.Mutate(x => x.Resize(width, height, Resampler));

        var bitmap = new byte[width * height * 4];
        image.CopyPixelDataTo(bitmap);

        return Svc.Texture.CreateFromRaw(RawImageSpecification.Bgra32(width, height), bitmap, $"ECommons.GameIcons {iconId}@{width}x{height}");
    }

    private static bool TryGetTexFile(GameIconLookup lookup, out TexFile file)
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

    private readonly record struct CacheKey(uint IconId, bool ItemHq, bool HiRes, ClientLanguage? Language, int Width, int Height);
}
