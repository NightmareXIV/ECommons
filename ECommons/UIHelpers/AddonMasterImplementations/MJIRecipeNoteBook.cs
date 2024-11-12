using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Island Sanctuary crafting log addon
    /// </summary>
    public unsafe class MJIRecipeNoteBook : AddonMasterBase<AtkUnitBase>
    {
        public MJIRecipeNoteBook(nint addon) : base(addon) { }
        public MJIRecipeNoteBook(void* addon) : base(addon) { }
        public AtkComponentButton* CraftButton => Addon->GetButtonNodeById(34);

        public override string AddonDescription => "Island Sanctuary crafting log window";

        public void Craft() => ClickButtonIfEnabled(CraftButton);
    }
}
