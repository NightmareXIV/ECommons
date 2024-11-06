using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Solo duty difficulty selection addon
    /// </summary>
    public unsafe class DifficultySelectYesNo : AddonMasterBase<AtkUnitBase>
    {
        public DifficultySelectYesNo(nint addon) : base(addon) { }

        public DifficultySelectYesNo(void* addon) : base(addon) { }

        public AtkComponentButton* ProceedButton => Addon->GetButtonNodeById(12);
        public AtkComponentButton* LeaveButton => Addon->GetButtonNodeById(13);

        public AtkComponentRadioButton* NormalButton => Addon->GetComponentNodeById(5)->GetAsAtkComponentRadioButton(); // which: 64
        public AtkComponentRadioButton* EasyButton => Addon->GetComponentNodeById(6)->GetAsAtkComponentRadioButton(); // which: 65
        public AtkComponentRadioButton* VeryEasyButton => Addon->GetComponentNodeById(7)->GetAsAtkComponentRadioButton(); // which: 66

        public override string AddonDescription { get; } = "Solo duty difficulty selection window";

        public void Proceed() => ClickButtonIfEnabled(ProceedButton);
        public void Leave() => ClickButtonIfEnabled(LeaveButton);

        // TODO: needs work
        public void SetDifficultyNormal() => ClickButtonIfEnabled(NormalButton);
        public void SetDifficultyEasy() => ClickButtonIfEnabled(EasyButton);
        public void SetDifficultyVeryEasy() => ClickButtonIfEnabled(VeryEasyButton);
    }
}
