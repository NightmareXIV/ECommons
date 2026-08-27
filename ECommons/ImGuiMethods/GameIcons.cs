using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Data.Files;
using System;
using System.Numerics;

namespace ECommons.ImGuiMethods;

public static class GameIcons
{
    private static volatile Func<TexFile, int, int, IDalamudTextureWrap?>? ResizeInternal;
    private static int MaximumSizeInternal = 512;

    public static Func<TexFile, int, int, IDalamudTextureWrap?>? Resize
    {
        get => ResizeInternal;
        set
        {
            if(ResizeInternal == value) return;
            ResizeInternal = value;
            ClearAll();
        }
    }

    public static int MaximumSize
    {
        get => MaximumSizeInternal;
        set
        {
            if(MaximumSizeInternal == value) return;
            MaximumSizeInternal = value;
            ClearAll();
        }
    }

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
        if(width <= 0 || height <= 0) return false;
        if(!TryGetScaledIcon(lookup, width, height, out var texture)) return false;
        var position = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(new Vector2(MathF.Round(position.X), MathF.Round(position.Y)));
        ImGui.Image(texture.Handle, new Vector2(width, height));
        return true;
    }

    public static bool TryGetScaledIcon(uint iconId, int size, out IDalamudTextureWrap texture) => TryGetScaledIcon(new GameIconLookup(iconId), size, size, out texture);

    public static bool TryGetScaledIcon(GameIconLookup lookup, int width, int height, out IDalamudTextureWrap texture) => ThreadLoadImageHandler.TryGetResampledIcon(lookup, width, height, out texture);

    public static void Invalidate(uint iconId) => ThreadLoadImageHandler.InvalidateIcon(iconId);

    public static void ClearAll() => ThreadLoadImageHandler.ClearIcons();
}
