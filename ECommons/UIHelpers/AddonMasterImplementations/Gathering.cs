using ECommons.Automation.UIInput;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Text.RegularExpressions;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe partial class Gathering : AddonMasterBase<AddonGathering>
    {
        public Gathering(nint addon) : base(addon) { }
        public Gathering(void* addon) : base(addon) { }

        public int CurrentIntegrity
        {
            get
            {
                var match = ExtractNumber().Match(Addon->GetTextNodeById(9)->NodeText.GetText());
                return match.Success ? int.Parse(match.Value) : 0;
            }
        }

        public int TotalIntegrity
        {
            get
            {
                var match = ExtractNumber().Match(Addon->GetTextNodeById(12)->NodeText.GetText());
                return match.Success ? int.Parse(match.Value) : 0;
            }
        }

        public GatheredItem[] GatheredItems
        {
            get
            {
                var gatheredItems = new GatheredItem[8];
                for(var i = 0; i < gatheredItems.Length; i++)
                {
                    gatheredItems[i] = new GatheredItem(this, Addon, GetCheckBox(i), i);
                }
                return gatheredItems;
            }
        }

        public override string AddonDescription { get; } = "Gathering window";

        public class GatheredItem
        {
            private Gathering addonMaster;
            private AddonGathering* addon;
            private AtkComponentCheckBox* checkbox;
            private int index;

            public GatheredItem(Gathering addonMaster, AddonGathering* addon, AtkComponentCheckBox* checkbox, int index)
            {
                this.addonMaster = addonMaster;
                this.addon = addon;
                this.checkbox = checkbox;
                this.index = index;
            }

            public AtkComponentCheckBox* CheckBox => checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public string ItemName => CheckBox->GetTextNodeById(23)->GetAsAtkTextNode()->NodeText.GetText();
            public uint ItemID => addon->ItemIds[index];
            public bool IsCollectable => Svc.Data.GetExcelSheet<Item>()?.GetRowOrDefault(ItemID)?.IsCollectable ?? false;
            public int ItemLevel
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(21)->GetAsAtkTextNode()->NodeText.GetText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }
            public int GatherChance
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(10)->GetAsAtkTextNode()->NodeText.GetText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }
            public int BoonChance
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(16)->GetAsAtkTextNode()->NodeText.GetText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }

            public void Gather() => addonMaster.ClickCheckboxIfEnabled(CheckBox);
        }

        private AtkComponentCheckBox* GetCheckBox(int index) => index switch
        {
            0 => Addon->GatheredItemComponentCheckbox[0],
            1 => Addon->GatheredItemComponentCheckbox[1],
            2 => Addon->GatheredItemComponentCheckbox[2],
            3 => Addon->GatheredItemComponentCheckbox[3],
            4 => Addon->GatheredItemComponentCheckbox[4],
            5 => Addon->GatheredItemComponentCheckbox[5],
            6 => Addon->GatheredItemComponentCheckbox[6],
            7 => Addon->GatheredItemComponentCheckbox[7],
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumber();
    }
}
