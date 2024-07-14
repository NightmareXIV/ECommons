﻿using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class JournalDetail : AddonMasterBase<AddonJournalDetail>
    {
        public JournalDetail(nint addon) : base(addon)
        {
        }

        public JournalDetail(void* addon) : base(addon) { }

        public void AcceptMap() => ClickButtonIfEnabled(Addon->AcceptMapButton);
        public void Initiate() => ClickButtonIfEnabled(Addon->InitiateButton);
        public void AbandonDecline() => ClickButtonIfEnabled(Addon->AbandonDeclineButton);
    }
}
