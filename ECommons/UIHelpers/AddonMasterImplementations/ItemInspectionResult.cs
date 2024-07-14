﻿using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ItemInspectionResult : AddonMasterBase<AddonItemInspectionResult>
    {
        public ItemInspectionResult(nint addon) : base(addon)
        {
        }

        public ItemInspectionResult(void* addon) : base(addon) { }

        public AtkTextNode* NameNode => Base->GetTextNodeById(26);
        public AtkTextNode* DescNode => Base->GetTextNodeById(35);
        public SeString ItemName => MemoryHelper.ReadSeString(&NameNode->NodeText);
        public SeString Description => MemoryHelper.ReadSeString(&DescNode->NodeText);
        public string ItemNameText => ItemName.ExtractText();
        public string DescriptionText => Description.ExtractText();

        public AtkComponentButton* NextButton => Base->GetButtonNodeById(73);
        public AtkComponentButton* CloseButton => Base->GetButtonNodeById(74);

        public void Next() => ClickButtonIfEnabled(NextButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
