using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System.Buffers.Binary;
using System.Numerics;

namespace ECommons.ImGuiMethods;

/// <summary>
/// Unified colour wrapper. Implicitly converts to needed formats and supports importing from most formats.
/// </summary>
/// <remarks>
/// <para>Contains built in redefinable colors (refinable after calling ECommonsMain.Init)</para>
/// </remarks>
public struct EzColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }
    public readonly Vector4 Vector4 => (Vector4)this;
    public readonly uint U32 => (uint)this;

    public EzColor() { }
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
    }

    public static EzColor From(float r, float g, float b, float a = 1)
        => new() { R = r, G = g, B = b, A = a };

    public static EzColor From(Vector3 col, float alpha = 1)
        => From(col.X, col.Y, col.Z, alpha);

    public static EzColor From(Vector4 col)
        => From(col.X, col.Y, col.Z, col.W);

    /// <summary>
    /// Takes in uints as 0xRRGGBB or 0xRRGGBBAA
    /// </summary>
    public static EzColor From(uint col)
        => From(ImGui.ColorConvertU32ToFloat4(AppendAlpha(col)));

    public static EzColor From(ImGuiCol col)
        => From(ImGui.GetColorU32(col));

    public static EzColor FromARGB(uint argb)
        => From((argb >> 24) | (argb << 8));

    public static EzColor FromABGR(uint abgr)
        => From(BinaryPrimitives.ReverseEndianness(abgr));

    public static EzColor FromUiForeground(uint id)
        => FromABGR(Svc.Data.GetExcelSheet<UIColor>().GetRow(id).Dark);

    public static EzColor FromUiGlow(uint id)
        => FromABGR(Svc.Data.GetExcelSheet<UIColor>().GetRow(id).Light);

    public static EzColor FromStain(uint id)
        => From(BinaryPrimitives.ReverseEndianness(Svc.Data.GetExcelSheet<Stain>().GetRow(id).Color) >> 8) with { A = 1 };

    public static implicit operator Vector4(EzColor col)
        => new(col.R, col.G, col.B, col.A);

    public static implicit operator uint(EzColor col)
        => ImGui.ColorConvertFloat4ToU32(col);

    private static uint AppendAlpha(uint col) => (col & 0xFFFFFF) == col ? (col << 8) | 0xFF : col;

    public static EzColor RedBright { get; set; } = new(0xFF0000);
    public static EzColor Red { get; set; } = new(0xAA0000);
    public static EzColor RedDark { get; set; } = new(0x440000);
    public static EzColor GreenBright { get; set; } = new(0x00ff00);
    public static EzColor Green { get; set; } = new(0x00aa00);
    public static EzColor GreenDark { get; set; } = new(0x004400);
    public static EzColor BlueBright { get; set; } = new(0x0000ff);
    public static EzColor Blue { get; set; } = new(0x0000aa);
    public static EzColor BlueSea { get; set; } = new(0x0058AA);
    public static EzColor BlueSky { get; set; } = new(0x0085FF);
    public static EzColor White { get; set; } = new(0xFFFFFF);
    public static EzColor Black { get; set; } = new(0x000000);
    public static EzColor Transparent { get; set; } = new(0x00000000);
    public static EzColor YellowBright { get; set; } = new(0xFFFF00);
    public static EzColor Yellow { get; set; } = new(0xAAAA00);
    public static EzColor YellowDark { get; set; } = new(0x444400);
    public static EzColor OrangeBright { get; set; } = new(0xFF7F00);
    public static EzColor Orange { get; set; } = new(0xAA5400);
    public static EzColor CyanBright { get; set; } = new(0x00FFFF);
    public static EzColor Cyan { get; set; } = new(0x00aaaa);
    public static EzColor VioletBright { get; set; } = new(0xFF00FF);
    public static EzColor Violet { get; set; } = new(0xAA00AA);
    public static EzColor VioletDark { get; set; } = new(0x440044);
    public static EzColor PurpleBright { get; set; } = new(0xFF0084);
    public static EzColor Purple { get; set; } = new(0xAA0058);
    public static EzColor PinkLight { get; set; } = new(0xFFABD6);
}