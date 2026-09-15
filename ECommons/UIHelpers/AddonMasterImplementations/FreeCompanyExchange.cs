using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public unsafe partial class AddonMaster
{
    public class FreeCompanyExchange : AddonMasterBase
    {
        public FreeCompanyExchange(nint addon) : base(addon)
        {
        }

        public FreeCompanyExchange(void* addon) : base(addon)
        {
        }

        public override string AddonDescription { get; } = "Free Company Action Shop";

        public NumberArrayData* Array => RaptureAtkModule.Instance()->AtkArrayDataHolder.GetNumberArrayData(61);

        public bool IsReady => Array->SubscribedAddonsCount > 0;

        public int Count => Array->IntArray[1];

        public List<Entry> Entries
        {
            get
            {
                var ret = new List<Entry>();
                for(int i = 0; i < Count; i++)
                {
                    ret.Add(new(this, i)
                    {
                        Action = CompanyAction.GetRef(Array->IntArray[2 + i * 4]),
                        RankRequired = Array->IntArray[4 + i * 4],
                        SealsRequired = Array->IntArray[5 + i * 4],
                    });
                }
                return ret;
            }
        }

        public class Entry(FreeCompanyExchange AddonMaster, int Index)
        {
            public RowRef<CompanyAction> Action;
            public int RankRequired;
            public int SealsRequired;

            public void Buy()
            {
                Callback.Fire(AddonMaster.Addon, true, 2, Index);
            }
        }
    }
}
