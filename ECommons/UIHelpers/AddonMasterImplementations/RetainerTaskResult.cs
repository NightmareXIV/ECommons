using ECommons.Automation.UIInput;
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
    public unsafe class RetainerTaskResult : AddonMasterBase<AddonRetainerTaskResult>
    {
        public RetainerTaskResult(nint addon) : base(addon)
        {
        }

        public RetainerTaskResult(void* addon) : base(addon)
        {
        }

        public AtkComponentButton* ReassignButton => Addon->ReassignButton;
        public AtkComponentButton* ConfirmButton => Addon->ConfirmButton;

        public override string AddonDescription { get; } = "Venture result window";

        public void Confirm() => ClickButtonIfEnabled(ConfirmButton);

        public void Reassign() => ClickButtonIfEnabled(ReassignButton);
    }
}
