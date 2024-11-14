using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectString : AddonMasterBase<AddonSelectString>
    {
        public SelectString(nint addon) : base(addon) { }
        public SelectString(void* addon) : base(addon) { }

        public int EntryCount => Addon->PopupMenu.PopupMenu.EntryCount;
        public SeString SeString => GenericHelpers.ReadSeString(&Base->GetTextNodeById(2)->NodeText);
        public string Text => SeString.ExtractText();

        public AtkComponentList* ListComponent => Addon->GetComponentListById(3);
        public List<Pointer<AtkComponentListItemRenderer>> ListItems
        {
            get
            {
                List<Pointer<AtkComponentListItemRenderer>> items = [];
                foreach (var node in Enumerable.Range(0, ListComponent->GetItemCount()))
                {
                    var item = ListComponent->GetItemRenderer(node);
                    if (item == null)
                        continue;
                    items.Add(item);
                }
                return items;
            }
        }

        public Entry[] Entries
        {
            get
            {
                var ret = new Entry[EntryCount];
                for (var i = 0; i < ret.Length; i++)
                {
                    ret[i] = new(this, Addon, i);
                }
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Selection menu";

        public struct Entry(SelectString am, AddonSelectString* addon, int index)
        {
            private readonly AddonSelectString* Addon = addon;
            public int Index { get; init; } = index;

            public readonly AtkTextNode* TextNode => am.ListItems[Index].Value->ButtonTextNode;
            public readonly SeString SeString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->PopupMenu.PopupMenu.EntryNames[Index]);
            public readonly string Text => SeString.ExtractText();

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
