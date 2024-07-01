using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation.UIInput;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class RecipeNote : AddonMasterBase<AddonRecipeNote>
    {
        public RecipeNote(nint addon) : base(addon)
        {
        }

        public void Material(uint index, bool hq)
        {
            var resNode = (AtkUnitBase*)Addon->GetNodeById(88);
            var subRes = (AtkUnitBase*)resNode->GetNodeById(89 + index);
            var btnRes = hq ? (AtkUnitBase*)subRes->GetNodeById(13) : (AtkUnitBase*)subRes->GetNodeById(10);
            var btn = hq ? btnRes->GetButtonNodeById(14) : btnRes->GetButtonNodeById(11);

            if (btn->IsEnabled)
            {
                btn->ClickAddonButton(Base);
            }
        }
    }
}
