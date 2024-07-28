using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
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