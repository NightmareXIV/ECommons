using FFXIVClientStructs.FFXIV.Component.Excel;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ExcelServices.Sheets;
//>:(
[Sheet]
public struct QuestDialogueText(ExcelPage page, uint offset, uint row) : IExcelRow<QuestDialogueText>
{
    public ReadOnlySeString Key => page.ReadString(offset, offset);
    public ReadOnlySeString Value => page.ReadString(offset + 4, offset);
    public uint RowId => row;

    public static QuestDialogueText Create(ExcelPage page, uint offset, uint row)
    {
        return new(page, offset, row);
    }
}
