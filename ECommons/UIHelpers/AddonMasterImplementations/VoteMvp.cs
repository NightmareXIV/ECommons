using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class VoteMvp : AddonMasterBase<AtkUnitBase>
    {
        public VoteMvp(nint addon) : base(addon) { }
        public VoteMvp(void* addon) : base(addon) { }

        public AtkComponentButton* OkButton => Addon->GetButtonNodeById(10);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(11);
        public AtkComponentButton* SettingsButton => Addon->GetButtonNodeById(12);

        public void Ok() => ClickButtonIfEnabled(OkButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
        public void Settings() => ClickButtonIfEnabled(SettingsButton);
    }
}
