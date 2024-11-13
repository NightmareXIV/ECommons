using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class PurifyResult : AddonMasterBase<AtkUnitBase>
    {
        public PurifyResult(nint addon) : base(addon) { }

        public PurifyResult(void* addon) : base(addon) { }

        public SeString BannerSeString => MemoryHelper.ReadSeString(&Base->GetTextNodeById(2)->NodeText);
        public string BannerText => BannerSeString.ExtractText();
        public AtkComponentButton* AutomaticButton => Addon->GetButtonNodeById(19);
        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(20);

        public override string AddonDescription { get; } = "Aetherial Reduction Result";

        public void Automatic() => ClickButtonIfEnabled(AutomaticButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
