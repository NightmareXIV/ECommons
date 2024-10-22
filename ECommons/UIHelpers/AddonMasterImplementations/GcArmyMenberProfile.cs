using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class GcArmyMenberProfile : AddonMasterBase<AtkUnitBase>
    {
        public GcArmyMenberProfile(nint addon) : base(addon) { }
        public GcArmyMenberProfile(void* addon) : base(addon) { }

        public AtkComponentButton* ViewMembersButton => Addon->GetButtonNodeById(2);
        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(3);

        public void ViewMembers() => ClickButtonIfEnabled(ViewMembersButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);

        public AtkComponentButton* QuestionButton => Addon->GetButtonNodeById(36);
        public AtkComponentButton* PostponeButton => Addon->GetButtonNodeById(37);
        public AtkComponentButton* DismissButton => Addon->GetButtonNodeById(38);

        public void Question() => ClickButtonIfEnabled(QuestionButton);
        public void Postpone() => ClickButtonIfEnabled(PostponeButton);
        public void Dismiss() => ClickButtonIfEnabled(DismissButton);

        public AtkComponentRadioButton* DisplayOrdersButton => Addon->GetComponentNodeById(31)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* ChangeClassButton => Addon->GetComponentNodeById(32)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* ConfirmChemistryButton => Addon->GetComponentNodeById(33)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* OutfitButton => Addon->GetComponentNodeById(34)->GetAsAtkComponentRadioButton();

        public override string AddonDescription { get; } = "Squadron member profile window";

        public void DisplayOrders() => ClickButtonIfEnabled(DisplayOrdersButton);
        public void ChangeClass() => ClickButtonIfEnabled(ChangeClassButton);
        public void ConfirmChemistry() => ClickButtonIfEnabled(ConfirmChemistryButton);
        public void Outfit() => ClickButtonIfEnabled(OutfitButton);
    }
}
