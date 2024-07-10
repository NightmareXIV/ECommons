using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageDialog : AddonMasterBase<AddonSalvageDialog>
    {
        public SalvageDialog(nint addon) : base(addon)
        {
        }

        public SalvageDialog(void* addon) : base(addon) { }

        public bool BulkDesynthEnabled { get => Addon->BulkDesynthEnabled; set => Addon->BulkDesynthEnabled = value; }

        public AtkComponentButton* DesynthesizeButton => Addon->GetButtonNodeById(24);

        public void Desynthesize() => ClickButtonIfEnabled(DesynthesizeButton);

        public void Checkbox()
        {
            if (Addon->CheckBox != null && !Addon->CheckBox->IsChecked)
                Callback.Fire(&Addon->AtkUnitBase, true, 13, true);
        }
    }
}
