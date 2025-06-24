using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class MiragePrismMiragePlateConfirm : AddonMasterBase
    {
        public MiragePrismMiragePlateConfirm(nint addon) : base(addon)
        {
        }

        public MiragePrismMiragePlateConfirm(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Dye confirmation window";

        public AtkComponentButton* YesButton => Base->GetComponentButtonById(6);
        public AtkComponentButton* NoButton => Base->GetComponentButtonById(7);

        public bool Yes() => ClickButtonIfEnabled(YesButton);
        public bool No() => ClickButtonIfEnabled(NoButton);
    }
}