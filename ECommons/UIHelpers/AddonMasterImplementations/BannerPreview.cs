using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class BannerPreview : AddonMasterBase<AtkUnitBase>
    {
        public BannerPreview(nint addon) : base(addon) { }
        public BannerPreview(void* addon) : base(addon) { }

        public AtkComponentButton* UpdateButton => Addon->GetButtonNodeById(8);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(9);
        public AtkComponentCheckBox* DoNotDisplayAgainCheckbox => Addon->GetComponentNodeById(2)->GetAsAtkComponentCheckBox();

        public override string AddonDescription => "Portrait Update Preview";

        public void Update() => ClickButtonIfEnabled(UpdateButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
