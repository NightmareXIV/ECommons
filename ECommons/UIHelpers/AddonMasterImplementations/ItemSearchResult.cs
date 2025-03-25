using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ItemSearchResult : AddonMasterBase<AddonItemSearchResult>
    {
        public ItemSearchResult(nint addon) : base(addon) { }
        public ItemSearchResult(void* addon) : base(addon) { }

        public AtkComponentList* ListComponent => Addon->Results;
        public List<Pointer<AtkComponentListItemRenderer>> ListItems
        {
            get
            {
                List<Pointer<AtkComponentListItemRenderer>> items = [];
                foreach(var node in Enumerable.Range(0, ListComponent->GetItemCount()))
                {
                    var item = ListComponent->GetItemRenderer(node);
                    if(item == null)
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
                var ret = new Entry[ListItems.Count];
                for(var i = 0; i < ret.Length; i++)
                    ret[i] = new(this, Addon, i);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Marketboard Item Listings";

        public readonly struct Entry(ItemSearchResult am, AddonItemSearchResult* addon, int index)
        {
            private readonly AddonItemSearchResult* Addon = addon;
            public readonly int Index { get; init; } = index;

            public AtkImageNode* HQImageNode => am.ListItems[Index].Value->ComponentNode->Component->GetImageNodeById(3)->GetAsAtkImageNode();
            public AtkTextNode* MateriaTextNode => am.ListItems[Index].Value->ComponentNode->Component->GetTextNodeById(4)->GetAsAtkTextNode();
            public AtkTextNode* PriceTextNode => am.ListItems[Index].Value->ComponentNode->Component->GetTextNodeById(5)->GetAsAtkTextNode();
            public AtkTextNode* QuantityTextNode => am.ListItems[Index].Value->ComponentNode->Component->GetTextNodeById(6)->GetAsAtkTextNode();
            public AtkTextNode* TotalTextNode => am.ListItems[Index].Value->ComponentNode->Component->GetTextNodeById(8)->GetAsAtkTextNode();
            public AtkTextNode* RetainerTextNode => am.ListItems[Index].Value->ComponentNode->Component->GetTextNodeById(10)->GetAsAtkTextNode();

            // TODO: Select function
            // Notes: A callback is sufficient if ItemSearchResult is from the marketboard. If it's your own listing, it requires a synthesised ListItemToggle event
        }
    }
}
