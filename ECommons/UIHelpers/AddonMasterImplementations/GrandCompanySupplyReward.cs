﻿using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.Automation.UIInput;

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

        public AtkComponentButton* DeliverButton => Base->GetButtonNodeById(38);

        public bool IsEnabled => DeliverButton->IsEnabled;

        public void Deliver()
        {
            if (IsEnabled) DeliverButton->ClickAddonButton(Base);
        }
    }
}