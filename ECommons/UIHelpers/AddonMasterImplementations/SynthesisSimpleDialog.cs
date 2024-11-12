using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SynthesisSimpleDialog : AddonMasterBase<AtkUnitBase>
    {
        public SynthesisSimpleDialog(nint addon) : base(addon) { }
        public SynthesisSimpleDialog(void* addon) : base(addon) { }

        public AtkComponentCheckBox* UseHQMaterialsCheckbox => Addon->GetComponentNodeById(5)->GetAsAtkComponentCheckBox();
        public AtkComponentButton* SynthesizeButton => Addon->GetButtonNodeById(7);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(8);

        public override string AddonDescription => "Quick synthesis in progress window";

        public void Synthesize() => ClickButtonIfEnabled(SynthesizeButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
