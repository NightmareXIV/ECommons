using ECommons.DalamudServices;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers;
public unsafe class AddonMaster
{
    public static IEnumerable<SelectStringMaster> SelectString
    {
        get
        {
            for (int i = 1; true; i++)
            {
                var addon = Svc.GameGui.GetAddonByName("SelectString", i);
                if (addon == 0) yield break;
                yield return new(addon);
            }
        }
    }

    public static IEnumerable<SelectYesnoMaster> YesNo
    {
        get
        {
            for (int i = 1; true; i++)
            {
                var addon = Svc.GameGui.GetAddonByName("SelectYesno", i);
                if (addon == 0) yield break;
                yield return new(addon);
            }
        }
    }
}
