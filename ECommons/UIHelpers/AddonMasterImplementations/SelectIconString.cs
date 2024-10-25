using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectIconString : AddonMasterBase<AddonSelectIconString>
    {
        public SelectIconString(nint addon) : base(addon) { }
        public SelectIconString(void* addon) : base(addon) { }

        public int EntryCount => Addon->PopupMenu.PopupMenu.EntryCount;
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
                    ret[i] = new(this, Addon, i);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "List selection menu with icons";

        public struct Entry(SelectIconString am, AddonSelectIconString* addon, int index)
        {
            private readonly AddonSelectIconString* Addon = addon;
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
                return $"AddonMaster.SelectIconString.Entry [Text=\"{Text}\", Index={Index}]";
            }
        }

        public void Entry1() => Entries[0].Select();
        public void Entry2() => Entries[1].Select();
        public void Entry3() => Entries[2].Select();
        public void Entry4() => Entries[3].Select();
        public void Entry5() => Entries[4].Select();
        public void Entry6() => Entries[5].Select();
        public void Entry7() => Entries[6].Select();
        public void Entry8() => Entries[7].Select();
        public void Entry9() => Entries[8].Select();
        public void Entry10() => Entries[9].Select();
        public void Entry11() => Entries[10].Select();
        public void Entry12() => Entries[11].Select();
    }
}
