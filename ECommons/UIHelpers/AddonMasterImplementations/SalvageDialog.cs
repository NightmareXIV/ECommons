using FFXIVClientStructs.FFXIV.Client.UI;

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

        public void Desynthesize() => ClickButtonIfEnabled(Addon->DesynthesizeButton);
        //public void Checkbox() => ClickCheckbox(Addon->CheckBox);
    }
}
