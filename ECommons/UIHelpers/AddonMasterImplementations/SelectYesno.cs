using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.Automation.UIInput;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectYesno : AddonMasterBase<AddonSelectYesno>
    {
        public SelectYesno(nint addon) : base(addon)
        {
        }

        public SelectYesno(void* addon) : base(addon) { }

        public SeString SeString => MemoryHelper.ReadSeString(&Addon->PromptText->NodeText);
        public string Text => SeString.ExtractText();

        public void Yes() => ClickButtonIfEnabled(Addon->YesButton);

        public void No() => ClickButtonIfEnabled(Addon->NoButton);
    }
}

[Obsolete("Please use AddonMaster.SelectYesno")]
public unsafe class SelectYesnoMaster : AddonMaster.SelectYesno
{
    public SelectYesnoMaster(nint addon) : base(addon)
    {
    }

    public SelectYesnoMaster(void* addon) : base(addon)
    {
    }
}