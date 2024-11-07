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

        public override string AddonDescription { get; } = "Materia creation window";

        public void Materialize() => ClickButtonIfEnabled(Addon->YesButton);
        public void No() => ClickButtonIfEnabled(Addon->NoButton);
    }
}