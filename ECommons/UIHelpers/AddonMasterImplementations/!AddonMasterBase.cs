using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe abstract class AddonMasterBase<T> where T : unmanaged
{
    protected AddonMasterBase(nint addon)
    {
        Addon = (T*)addon;
    }
    protected AddonMasterBase(void* addon)
    {
        Addon = (T*)addon;
    }

    public T* Addon { get; }
    public AtkUnitBase* Base => (AtkUnitBase*)Addon;
    public bool IsVisible => Base->IsVisible;
    public bool IsReady => GenericHelpers.IsAddonReady(Base);

    protected bool ClickButtonIfEnabled(AtkComponentButton* button)
    {
        if (button->IsEnabled)
        {
            button->ClickAddonButton(Base);
            return true;
        }
        return false;
    }
}
