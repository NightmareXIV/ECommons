using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MateriaRetrieveDialog : AddonMasterBase<AddonMateriaRetrieveDialog>
    {
        public MateriaRetrieveDialog(nint addon) : base(addon)
        {
        }

        public MateriaRetrieveDialog(void* addon) : base(addon) { }

        public AtkComponentButton* BeginButton => Base->GetButtonNodeById(17);
        public AtkComponentButton* ReturnButton => Base->GetButtonNodeById(18);

        public override string AddonDescription { get; } = "Materia extraction window";

        public void Begin() => ClickButtonIfEnabled(BeginButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}
