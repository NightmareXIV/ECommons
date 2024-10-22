using ECommons.Automation;
using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class RecipeNote : AddonMasterBase<AddonRecipeNote>
    {
        public override string AddonDescription { get; } = "Crafting Log";

        public RecipeNote(nint addon) : base(addon)
        {
        }

        public RecipeNote(void* addon) : base(addon) { }

        public void Synthesize() => ClickButtonIfEnabled(Addon->SynthesizeButton);

        public void QuickSynthesis() => ClickButtonIfEnabled(Addon->QuickSynthesisButton);

        public void TrialSynthesis() => ClickButtonIfEnabled(Addon->TrialSynthesisButton);

        public void Material(uint index, bool hq)
        {
            if(hq) index += 0x10_000;
            Callback.Fire(Base, false, 6, index, 0);
        }
    }
}
