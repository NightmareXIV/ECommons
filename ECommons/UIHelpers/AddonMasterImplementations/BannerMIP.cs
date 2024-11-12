using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Portrait MVP voting addon
    /// </summary>
    public unsafe class BannerMIP : AddonMasterBase<AtkUnitBase>
    {
        public BannerMIP(nint addon) : base(addon) { }
        public BannerMIP(void* addon) : base(addon) { }

        public AtkComponentButton* OkButton => Addon->GetButtonNodeById(20);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(21);
        public AtkComponentButton* SettingsButton => Addon->GetButtonNodeById(22);

        public override string AddonDescription => "Player commendation voting (with portraits) window";

        public void Ok() => ClickButtonIfEnabled(OkButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
        public void Settings() => ClickButtonIfEnabled(SettingsButton);
    }
}
