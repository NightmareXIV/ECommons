using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class VoteMvp : AddonMasterBase<AtkUnitBase>
    {
        public VoteMvp(nint addon) : base(addon) { }
        public VoteMvp(void* addon) : base(addon) { }

        public AtkComponentButton* OkButton => Addon->GetComponentButtonById(10);
        public AtkComponentButton* CancelButton => Addon->GetComponentButtonById(11);
        public AtkComponentButton* SettingsButton => Addon->GetComponentButtonById(12);

        public override string AddonDescription { get; } = "Player commendations simple voting window";

        public void Ok() => ClickButtonIfEnabled(OkButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
        public void Settings() => ClickButtonIfEnabled(SettingsButton);
    }
}
