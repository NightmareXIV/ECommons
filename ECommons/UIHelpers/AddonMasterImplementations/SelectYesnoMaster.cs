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
public unsafe class SelectYesnoMaster : AddonMasterBase<AddonSelectYesno>
{
    public SelectYesnoMaster(nint addon) : base(addon)
    {
    }

    public SelectYesnoMaster(void* addon) : base(addon) { }

    public SeString SeString => MemoryHelper.ReadSeString(&Addon->PromptText->NodeText);
    public string Text => SeString.ExtractText();

    public void Yes()
    {
        var btn = Addon->YesButton;
        if (btn->IsEnabled)
        {
            btn->ClickAddonButton(Base);
        }
    }

    public void No()
    {
        var btn = Addon->NoButton;
        if (btn->IsEnabled)
        {
            btn->ClickAddonButton(Base);
        }
    }
}
