using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe class MaterializeDialogMaster : AddonMasterBase<AddonMaterializeDialog>
{
    public MaterializeDialogMaster(nint addon) : base(addon)
    {
    }

    public MaterializeDialogMaster(void* addon) : base(addon) { }

    public void Materialize()
    {
        var btn = Addon->YesButton;
        if (btn->IsEnabled)
        {
            btn->ClickAddonButton(Base);
        }
    }
}

