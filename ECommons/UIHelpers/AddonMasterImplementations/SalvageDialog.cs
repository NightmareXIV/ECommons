using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageDialog : AddonMasterBase<AddonSalvageDialog>
    {
        public SalvageDialog(nint addon) : base(addon) { }
        public SalvageDialog(void* addon) : base(addon) { }

        // this has no bearing on the checkbox
        public bool BulkDesynthEnabled { get => Addon->BulkDesynthEnabled; set => Addon->BulkDesynthEnabled = value; }

        public AtkComponentButton* DesynthesizeButton => Addon->GetComponentButtonById(25);
        public AtkComponentButton* CancelButton => Addon->GetComponentButtonById(26);

        public override string AddonDescription { get; } = "Desynthesis window";

        public void Desynthesize() => ClickButtonIfEnabled(DesynthesizeButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);

        [Obsolete("Do not use")]
        public void Checkbox()
        {
            // do nothing for now
        }
    }
}
