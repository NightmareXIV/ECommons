using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using Lumina.Text.ReadOnly;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ContextMenu : AddonMasterBase<AddonContextMenu>
    {
        public ContextMenu(nint addon) : base(addon) { }
        public ContextMenu(void* addon) : base(addon) { }

        public int EntriesCount => (int)Addon->AtkValues[0].UInt;

        public AtkComponentList* ListComponent => Addon->GetComponentListById(2);
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

        private const int offset = 7;
        public Entry[] Entries
        {
            get
            {
                var ret = new Entry[EntriesCount];
                for (var i = 0; i < ret.Length; i++)
                    ret[i] = new(this, Addon, i);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Context menu";

        public readonly struct Entry(ContextMenu am, AddonContextMenu* addon, int index)
        {
            private readonly AddonContextMenu* Addon = addon;
            public readonly int Index { get; init; } = index;
            public readonly int ListIndex => Index + offset;
            // Dalamud added context menu entries all have a callback index of -1, which results in looping the list and calling something else. AFAIK, native entries are always a single payload of rawtext.
            public readonly bool IsNativeEntry => Addon->AtkValues[ListIndex].Type == FFXIVClientStructs.FFXIV.Component.GUI.ValueType.ManagedString && new ReadOnlySeStringSpan(((AtkValue*)(nint)(&Addon->AtkValues[ListIndex]))->String).PayloadCount == 1;

            public AtkTextNode* TextNode => am.ListItems[Index].Value->ButtonTextNode;
            public readonly SeString SeString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[ListIndex].String);
            public readonly string Text => SeString.ExtractText();
            public readonly bool Enabled => am.ListItems[Index].Value->IsEnabled;

            public readonly bool Select()
            {
                if (IsNativeEntry && Enabled)
                {
                    Callback.Fire((AtkUnitBase*)Addon, true, 0, Index, 0);
                    return true;
                }
                return false;
            }

            public override readonly string? ToString() => $"{nameof(AddonMaster)}.{nameof(ContextMenu)}.{nameof(Entry)} [Text=\"{Text}\", Index={ListIndex} CallbackIndex={Index}]";
        }
    }
}
