using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace ECommons.ExcelServices.Sheets;

//This code is authored by Caraxi https://github.com/Caraxi/SimpleTweaksPlugin/
[Sheet("BaseParam")]
public readonly unsafe struct ExtendedBaseParam(ExcelPage page, uint offset, uint row) : IRowExtension<ExtendedBaseParam, BaseParam>
{

    private const int ParamCount = 23;

    public BaseParam BaseParam => new(page, offset, row);
    public Collection<ushort> EquipSlotCategoryPct => new(page, offset, offset, &EquipSlotCategoryPctCtor, ParamCount);
    private static ushort EquipSlotCategoryPctCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => i == 0 ? (ushort)0 : page.ReadUInt16(offset + 8 + (i - 1) * 2);
    public static ExtendedBaseParam Create(ExcelPage page, uint offset, uint row) => new(page, offset, row);
    public uint RowId => row;
}