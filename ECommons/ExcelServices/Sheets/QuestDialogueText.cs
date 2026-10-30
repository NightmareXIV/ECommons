using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace ECommons.ExcelServices.Sheets;
//>:(
[Sheet]
public struct QuestDialogueText(ExcelPage page, uint offset, uint row) : IExcelRow<QuestDialogueText>
{
    public ExcelPage ExcelPage => page;
    public ReadOnlySeString Key => page.ReadString(offset, offset);
    public ReadOnlySeString Value => page.ReadString(offset + 4, offset);
    public uint RowOffset => offset;
    public uint RowId => row;

    public static QuestDialogueText Create(ExcelPage page, uint offset, uint row)
    {
        return new(page, offset, row);
    }
}
