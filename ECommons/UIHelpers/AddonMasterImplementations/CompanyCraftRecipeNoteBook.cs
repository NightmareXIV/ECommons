using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class CompanyCraftRecipeNoteBook : AddonMasterBase<AtkUnitBase>
    {
        public CompanyCraftRecipeNoteBook(nint addon) : base(addon) { }
        public CompanyCraftRecipeNoteBook(void* addon) : base(addon) { }

        public AtkComponentButton* BeginButton => Base->GetButtonNodeById(34);

        public override string AddonDescription { get; } = "Free Company crafting log";

        public void Begin() => ClickButtonIfEnabled(BeginButton);
    }
}