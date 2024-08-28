using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectString : AddonMasterBase<AddonSelectString>
    {
        public SelectString(nint addon) : base(addon)
        {
        }

        public SelectString(void* addon) : base(addon) { }

        public SeString SeString => MemoryHelper.ReadSeString(&Base->GetTextNodeById(2)->NodeText);
        public string Text => SeString.ExtractText();

        public Entry[] Entries
        {
            get
            {
                var ret = new Entry[Addon->PopupMenu.PopupMenu.EntryCount];
                for(var i = 0; i < ret.Length; i++)
                {
                    ret[i] = new(Addon, i);
                }
                return ret;
            }
        }

        public struct Entry
        {
            private AddonSelectString* Addon;
            public int Index { get; init; }

            public Entry(AddonSelectString* addon, int index)
            {
                Addon = addon;
                Index = index;
            }

            public SeString SeString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->PopupMenu.PopupMenu.EntryNames[Index]);
            public string Text => SeString.ExtractText();

            public readonly void Select()
            {
                Callback.Fire((AtkUnitBase*)Addon, true, Index);
            }

            public override string? ToString()
            {
                return $"AddonMaster.SelectString.Entry [Text=\"{Text}\", Index={Index}]";
            }
        }

        private void Entry1() => Entries[0].Select();
        private void Entry2() => Entries[1].Select();
        private void Entry3() => Entries[2].Select();
        private void Entry4() => Entries[3].Select();
        private void Entry5() => Entries[4].Select();
        private void Entry6() => Entries[5].Select();
        private void Entry7() => Entries[6].Select();
        private void Entry8() => Entries[7].Select();
        private void Entry9() => Entries[8].Select();
        private void Entry10() => Entries[9].Select();
        private void Entry11() => Entries[10].Select();
        private void Entry12() => Entries[11].Select();
    }
}

[Obsolete("Please use AddonMaster.SelectString")]
public unsafe class SelectStringMaster : AddonMaster.SelectString
{
    public SelectStringMaster(nint addon) : base(addon)
    {
    }

    public SelectStringMaster(void* addon) : base(addon)
    {
    }
}
