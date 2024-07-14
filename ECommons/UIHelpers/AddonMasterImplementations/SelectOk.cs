﻿using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectOk : AddonMasterBase<AddonSelectOk>
    {
        public SelectOk(nint addon) : base(addon)
        {
        }

        public SelectOk(void* addon) : base(addon) { }

        public SeString SeString => MemoryHelper.ReadSeString(&Addon->PromptText->NodeText);
        public string Text => SeString.ExtractText();

        public void Ok() => ClickButtonIfEnabled(Addon->OkButton);
    }
}
