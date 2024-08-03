using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class _TitleMenu : AddonMasterBase
    {
        public _TitleMenu(nint addon) : base(addon)
        {
        }

        public _TitleMenu(void* addon) : base(addon)
        {
        }

        public bool IsReady
        {
            get
            {
                return GenericHelpers.IsScreenReady()
                    && GenericHelpers.IsAddonReady(Base)
                    && Base->UldManager.NodeListCount > 3
                    && Base->UldManager.NodeList[7]->IsVisible()
                    && Base->GetNodeById(3)->Color.A == 0xFF
                    && !GenericHelpers.TryGetAddonByName<AtkUnitBase>("TitleDCWorldMap", out _)
                    && !GenericHelpers.TryGetAddonByName<AtkUnitBase>("TitleConnect", out _);
            }
        }

        public void Start() => ClickButtonIfEnabled(Base->GetButtonNodeById(4));
        public void DataCenter() => ClickButtonIfEnabled(Base->GetButtonNodeById(5));
        public void Exit() => ClickButtonIfEnabled(Base->GetButtonNodeById(9));
    }
}
