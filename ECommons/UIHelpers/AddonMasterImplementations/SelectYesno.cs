using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectYesno : AddonMasterBase<AddonSelectYesno>
    {
        public SelectYesno(nint addon) : base(addon) { }
        public SelectYesno(void* addon) : base(addon) { }

        public SeString SeString => GenericHelpers.ReadSeString(&Addon->PromptText->NodeText);
        public SeString SeStringNullTerminated => MemoryHelper.ReadSeStringNullTerminated(new nint(Addon->AtkValues[0].String));
        public string Text => SeString.GetText();
        public string TextLegacy => string.Join(string.Empty, SeStringNullTerminated.Payloads.OfType<TextPayload>().Select(t => t.Text)).Replace('\n', ' ').Trim();
        public int ButtonsVisible => Enumerable.Range(1, 3).Count(x => Addon->AtkValues[x].String != null);

        public AtkComponentButton* ThirdButton => Addon->GetButtonNodeById(14);

        public override string AddonDescription { get; } = "Yes or No selection menu";

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

        /// <summary>
        /// This is always the second button. In a two button SelectYesno, this is no. In a three button SelectYesno, it can be something else (such as "Wait")
        /// </summary>
        public void No() => ClickButtonIfEnabled(Addon->NoButton);
        public void Third() => ClickButtonIfEnabled(ThirdButton);
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
