using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.Implementations
{
    public unsafe class ReaderRetainerSellList(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
    {
        public List<Entry> Entries => Loop<Entry>(10, 10, 20);


        public unsafe class Entry(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
        {
            public int IconId => ReadInt(0) ?? 0;
            public string Name => ReadSeString(1).ExtractText(true);
            public int Amount => ReadInt(2) ?? 0;
            public int PricePerUnit => int.TryParse(ReadString(3).RemoveOtherChars("0123456789"), out var ret) ? ret : 0;
        }
    }
}
