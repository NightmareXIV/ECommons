using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
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
                var match = ExtractNumber().Match(Addon->GetTextNodeById(9)->NodeText.ExtractText());
                return match.Success ? int.Parse(match.Value) : 0;
            }
        }

        public int TotalIntegrity
        {
            get
            {
                var match = ExtractNumber().Match(Addon->GetTextNodeById(12)->NodeText.ExtractText());
                return match.Success ? int.Parse(match.Value) : 0;
            }
        }

        public GatheredItem[] GatheredItems
        {
            get
            {
                GatheredItem[] gatheredItems = new GatheredItem[8];
                for (int i = 0; i < gatheredItems.Length; i++)
                {
                    gatheredItems[i] = new GatheredItem(this, Addon, GetCheckBox(i), i);
                }
                return gatheredItems;
            }
        }

        public class GatheredItem(Gathering addonMaster, AddonGathering* addon, AtkComponentCheckBox* checkbox, int index)
        {
            public AtkComponentCheckBox* CheckBox = checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public string ItemName => CheckBox->GetTextNodeById(23)->GetAsAtkTextNode()->NodeText.ExtractText();
            public uint ItemID => addon->ItemIds[index];
            public bool IsCollectable => Svc.Data.GetExcelSheet<Item>()?.GetRow(ItemID)?.IsCollectable ?? false;
            public int ItemLevel
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(21)->GetAsAtkTextNode()->NodeText.ExtractText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }
            public int GatherChance
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(10)->GetAsAtkTextNode()->NodeText.ExtractText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }
            public int BoonChance
            {
                get
                {
                    var match = ExtractNumber().Match(CheckBox->GetTextNodeById(16)->GetAsAtkTextNode()->NodeText.ExtractText());
                    return match.Success ? int.Parse(match.Value) : 0;
                }
            }

            public void Gather()
            {
                if (IsEnabled)
                {
                    var evt = CheckBox->OwnerNode->AtkResNode.AtkEventManager.Event;
                    var data = stackalloc AtkEventData[1];
                    addon->AtkUnitBase.ReceiveEvent(evt->Type, (int)evt->Param, evt, data);
                }
            }
        }

        private AtkComponentCheckBox* GetCheckBox(int index) => index switch
        {
            0 => Addon->GatheredItemComponentCheckBox1,
            1 => Addon->GatheredItemComponentCheckBox2,
            2 => Addon->GatheredItemComponentCheckBox3,
            3 => Addon->GatheredItemComponentCheckBox4,
            4 => Addon->GatheredItemComponentCheckBox5,
            5 => Addon->GatheredItemComponentCheckBox6,
            6 => Addon->GatheredItemComponentCheckBox7,
            7 => Addon->GatheredItemComponentCheckBox8,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumber();
    }
}
