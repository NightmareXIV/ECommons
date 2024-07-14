﻿using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster{
    public class TitleDCWorldMap : AddonMasterBase
    {
        public TitleDCWorldMap(nint addon) : base(addon)
        {
        }

        public TitleDCWorldMap(void* addon) : base(addon)
        {
        }

        public static int[] PublicDC = [1, 2, 3, 10, 4, 5, 8, 11, 6, 7, 12, 9,];

        public bool Select(int dc)
        {
            if (!PublicDC.Contains(dc)) return false;
            var data = Svc.Data.GetExcelSheet<WorldDCGroupType>()?.GetRow((uint)dc);
            if(data != null)
            {
                if (data.Name.ExtractText().IsNullOrEmpty()) return false;
                if (data.Region == 0) return false;
                UncheckedSelect(dc);
                return true;
            }
            return false;
        }

        public void UncheckedSelect(int dc)
        {
            Callback.Fire(Base, true, 17, dc);
        }
    }
}
