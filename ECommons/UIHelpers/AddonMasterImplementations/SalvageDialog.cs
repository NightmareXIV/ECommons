using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageDialog : AddonMasterBase<AddonSalvageDialog>
    {
        public SalvageDialog(nint addon) : base(addon) { }
        public SalvageDialog(void* addon) : base(addon) { }

        // this has no bearing on the checkbox
        public bool BulkDesynthEnabled { get => Addon->BulkDesynthEnabled; set => Addon->BulkDesynthEnabled = value; }

        public AtkComponentButton* DesynthesizeButton => Addon->GetButtonNodeById(24);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(25);

        public override string AddonDescription { get; } = "Desynthesis window";

        public void Desynthesize() => ClickButtonIfEnabled(DesynthesizeButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);

        public void Checkbox()
        {
            if(Addon->BulkDesynthCheckboxNode != null && !Addon->BulkDesynthCheckboxNode->IsChecked)
            {
                Addon->BulkDesynthCheckboxNode->IsChecked = true;
                DesynthesizeButton->SetEnabledState(true);
            }
        }
    }
}
