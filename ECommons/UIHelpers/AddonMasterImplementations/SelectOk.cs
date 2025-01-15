using Dalamud.Game.Text.SeStringHandling;
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

        public SeString SeString => GenericHelpers.ReadSeString(&Addon->PromptText->NodeText);
        public string Text => SeString.GetText();

        public override string AddonDescription { get; } = "Generic confirmation window (OK button)";

        public void Ok() => ClickButtonIfEnabled(Addon->OkButton);
    }
}
