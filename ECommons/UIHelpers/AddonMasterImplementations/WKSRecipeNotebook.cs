using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    /// <summary>
    /// Moon Recipe Notebook
    /// Works similary to the normal recipe, except it's specialized to moon items you're doing the quest for
    /// </summary>
    public unsafe partial class WKSRecipeNotebook : AddonMasterBase<AtkUnitBase>
    {
        public WKSRecipeNotebook(nint addon) : base(addon) { }
        public WKSRecipeNotebook(void* addon) : base(addon) { }

        public AtkComponentButton* NQItemsButton => Addon->GetButtonNodeById(39);
        public AtkComponentButton* HQItemsButton => Addon->GetButtonNodeById(40);
        public AtkComponentButton* SynthesizeButton => Addon->GetButtonNodeById(50);

        public string SelectedCraftingItem => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[46].String.Value).GetText();

        public CraftItems[] CraftingItems
        {
            get
            {
                var ret = new List<CraftItems>();
                for(var i = 0; i < 5; i++)
                {
                    var itemName = Addon->AtkValues[35 + i * 2];
                    if(itemName.Type.EqualsAny(ValueType.String, ValueType.ManagedString, ValueType.String8))
                    {
                        var item = new CraftItems(this, i)
                        {
                            Name = MemoryHelper.ReadSeStringNullTerminated((nint)itemName.String.Value).GetText()
                        };
                        ret.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
                return [.. ret];

            }
        }

        public class CraftItems(WKSRecipeNotebook master, int index)
        {
            public string Name;

            public void Select()
            {
                Callback.Fire(master.Base, true, 0, index);
            }
        }

        public void NQItemInput() => ClickButtonIfEnabled(NQItemsButton);
        public void HQItemInput() => ClickButtonIfEnabled(HQItemsButton);
        public void Synthesize() => ClickButtonIfEnabled(SynthesizeButton);

        public override string AddonDescription => "Crafting Addon for Cosmic Exploration";
    }
}
