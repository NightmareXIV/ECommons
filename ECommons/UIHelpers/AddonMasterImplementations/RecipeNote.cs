using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation.UIInput;
using ECommons.Automation;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class RecipeNote : AddonMasterBase<AddonRecipeNote>
    {
        public RecipeNote(nint addon) : base(addon)
        {
        }

        public RecipeNote(void* addon) : base(addon) { }

        public void Synthesize()
        {
            var btn = Addon->SynthesizeButton;
            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }

        public void QuickSynthesis()
        {
            var btn = Addon->QuickSynthesisButton;
            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }

        public void TrialSynthesis()
        {
            var btn = Addon->TrialSynthesisButton;
            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }

        public void Material(uint index, bool hq)
        {
            if (hq) index += 0x10_000;
            Callback.Fire(Base, false, 6, index, 0);
        }
    }
}
