using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Linq;

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
        public SeString SeStringNullTerminated => MemoryHelper.ReadSeStringNullTerminated(new nint(Addon->AtkValues[0].String));
        public string Text => SeString.ExtractText();
        public string TextLegacy => string.Join(string.Empty, SeStringNullTerminated.Payloads.OfType<TextPayload>().Select(t => t.Text)).Replace('\n', ' ').Trim();

        public void Yes()
        {
            if(Addon->YesButton != null && !Addon->YesButton->IsEnabled)
            {
                Svc.Log.Debug($"{nameof(AddonSelectYesno)}: Force enabling yes button");
                var flagsPtr = (ushort*)&Addon->YesButton->AtkComponentBase.OwnerNode->AtkResNode.NodeFlags;
                *flagsPtr ^= 1 << 5;
            }
            ClickButtonIfEnabled(Addon->YesButton);
        }

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
