using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Buffers.Binary;
using System.Numerics;

namespace ECommons.ImGuiMethods;

/// <summary>
/// Unified colour wrapper. Implicitly converts to needed formats and supports importing from most formats.
/// </summary>
/// <remarks>
/// <para>Contains built in redefinable colors (refinable after calling ECommonsMain.Init)</para>
/// </remarks>
public readonly struct EzColor
{
    public float R { get; init; }
    public float G { get; init; }
    public float B { get; init; }
    public float A { get; init; }
    public readonly Vector4 Vector4;
    public readonly uint U32;

    public EzColor()
    {
        Vector4 = new(R, G, B, A);
        U32 = ImGui.ColorConvertFloat4ToU32(Vector4);
    }
    public EzColor(Vector4 vec) : this(vec.X, vec.Y, vec.Z, vec.W) { }
    /// <summary>
    /// Takes in uints as 0xRRGGBB or 0xRRGGBBAA
    /// </summary>
    public EzColor(uint col) : this(ImGui.ColorConvertU32ToFloat4(AppendAlpha(col))) { }
    public EzColor(float r, float g, float b, float a = 1)
    {
        R = r;
        G = g;
        B = b;
        A = a;
        Vector4 = new(R, G, B, A);
        U32 = ImGui.ColorConvertFloat4ToU32(Vector4);
    }

    public static EzColor From(float r, float g, float b, float a = 1)
    {
        return new(r, g, b, a);
    }

    public static EzColor From(Vector3 col, float alpha = 1)
    {
        return From(col.X, col.Y, col.Z, alpha);
    }

    public static EzColor From(Vector4 col)
    {
        return From(col.X, col.Y, col.Z, col.W);
    }

    /// <summary>
    /// Takes in uints as 0xRRGGBB or 0xRRGGBBAA
    /// </summary>
    public static EzColor From(uint col)
    {
        return From(ImGui.ColorConvertU32ToFloat4(AppendAlpha(col)));
    }

    public static EzColor From(ImGuiCol col)
    {
        return From(ImGui.GetColorU32(col));
    }

    public static EzColor FromARGB(uint argb)
    {
        return From((argb >> 24) | (argb << 8));
    }

    public static EzColor FromABGR(uint abgr)
    {
        return From(BinaryPrimitives.ReverseEndianness(abgr));
    }

    [Obsolete("Verify")]
    public static EzColor FromUiForeground(uint id)
    {
        return FromABGR(Svc.Data.GetExcelSheet<UIColor>().GetRow(id).Dark);
    }

    [Obsolete("Verify")]
    public static EzColor FromUiGlow(uint id)
    {
        return FromABGR(Svc.Data.GetExcelSheet<UIColor>().GetRow(id).Light);
    }

    public static EzColor FromStain(uint id)
    {
        return From(BinaryPrimitives.ReverseEndianness(Svc.Data.GetExcelSheet<Stain>().GetRow(id).Color) >> 8) with { A = 1 };
    }

    public static implicit operator Vector4(EzColor col)
    {
        return new(col.R, col.G, col.B, col.A);
    }

    public static implicit operator uint(EzColor col)
    {
        return ImGui.ColorConvertFloat4ToU32(col);
    }

    private static uint AppendAlpha(uint col)
    {
        return (col & 0xFFFFFF) == col ? (col << 8) | 0xFF : col;
    }

    public static EzColor RedBright = new(0xFF0000);
    public static EzColor Red = new(0xAA0000);
    public static EzColor RedDark = new(0x440000);
    public static EzColor GreenBright = new(0x00ff00);
    public static EzColor Green = new(0x00aa00);
    public static EzColor GreenDark = new(0x004400);
    public static EzColor BlueBright = new(0x0000ff);
    public static EzColor Blue = new(0x0000aa);
    public static EzColor BlueSea = new(0x0058AA);
    public static EzColor BlueSky = new(0x0085FF);
    public static EzColor White = new(0xFFFFFF);
    public static EzColor Black = new(0x000000);
    public static EzColor Transparent = new(0x00000000);
    public static EzColor YellowBright = new(0xFFFF00);
    public static EzColor Yellow = new(0xAAAA00);
    public static EzColor YellowDark = new(0x444400);
    public static EzColor OrangeBright = new(0xFF7F00);
    public static EzColor Orange = new(0xAA5400);
    public static EzColor CyanBright = new(0x00FFFF);
    public static EzColor Cyan = new(0x00aaaa);
    public static EzColor VioletBright = new(0xFF00FF);
    public static EzColor Violet = new(0xAA00AA);
    public static EzColor VioletDark = new(0x440044);
    public static EzColor PurpleBright = new(0xFF0084);
    public static EzColor Purple = new(0xAA0058);
    public static EzColor PinkLight = new(0xFFABD6);
}
