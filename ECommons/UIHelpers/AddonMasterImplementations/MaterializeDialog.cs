using FFXIVClientStructs.FFXIV.Client.UI;
using System;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MaterializeDialog : AddonMasterBase<AddonMaterializeDialog>
    {
        public MaterializeDialog(nint addon) : base(addon)
        {
        }

        public MaterializeDialog(void* addon) : base(addon) { }

        public void Materialize() => ClickButtonIfEnabled(Addon->YesButton);
        public void No() => ClickButtonIfEnabled(Addon->NoButton);
    }
}

[Obsolete("Please use AddonMaster.MaterializeDialog")]
public unsafe class MaterializeDialogMaster : AddonMaster.MaterializeDialog
{
    public MaterializeDialogMaster(nint addon) : base(addon)
    {
    }

    public MaterializeDialogMaster(void* addon) : base(addon)
    {
    }
}