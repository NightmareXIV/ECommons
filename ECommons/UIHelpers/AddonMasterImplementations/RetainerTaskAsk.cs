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
    public unsafe class RetainerTaskAsk : AddonMasterBase<AddonRetainerTaskAsk>
    {
        public RetainerTaskAsk(nint addon) : base(addon)
        {
        }

        public RetainerTaskAsk(void* addon) : base(addon)
        {
        }

        public AtkComponentButton* AssignButton => Addon->AssignButton;
        public AtkComponentButton* ReturnButton => Addon->ReturnButton;

        public override string AddonDescription { get; } = "Venture assign window";

        public void Assign() => ClickButtonIfEnabled(AssignButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}