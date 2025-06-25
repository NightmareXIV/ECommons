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
    public unsafe class GrandCompanySupplyReward : AddonMasterBase<AddonGrandCompanySupplyReward>
    {
        public GrandCompanySupplyReward(nint addon) : base(addon)
        {
        }

        public GrandCompanySupplyReward(void* addon) : base(addon)
        {
        }

        public AtkComponentButton* DeliverButton => Base->GetComponentButtonById(38);

        public bool IsEnabled => DeliverButton->IsEnabled;

        public override string AddonDescription { get; } = "Grand Company delivery window";

        public void Deliver() => ClickButtonIfEnabled(DeliverButton);
    }
}