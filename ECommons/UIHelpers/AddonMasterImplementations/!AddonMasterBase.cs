using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public abstract unsafe class AddonMasterBase<T> : IAddonMasterBase where T : unmanaged
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
    public bool IsAddonReady => GenericHelpers.IsAddonReady(Base);
    public bool IsAddonFocused
    {
        get
        {
            for (var i = 0; i < RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count; i++)
            {
                var atk = RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[i].Value;
                if (atk != null && atk == Base) return true;
            }
            return false;
        }
    }

    public bool IsAddonHighestFocus => RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count - 1].Value == Base;

    protected bool ClickButtonIfEnabled(AtkComponentButton* button)
    {
        if(button->IsEnabled && button->AtkResNode->IsVisible())
        {
            button->ClickAddonButton(Base);
            return true;
        }
        return false;
    }

    protected bool ClickButtonIfEnabled(AtkComponentRadioButton* button)
    {
        if (button->IsEnabled && button->AtkResNode->IsVisible())
        {
            button->ClickRadioButton(Base);
            return true;
        }
        return false;
    }

    protected AtkEvent CreateAtkEvent(byte flags = 0)
    {
        return new()
        {
            Listener = (AtkEventListener*)Base,
            Flags = flags,
            Target = &AtkStage.Instance()->AtkEventTarget
        };
    }

    protected AtkEventDataBuilder CreateAtkEventData()
    {
        return new();
    }
}

public abstract unsafe class AddonMasterBase : AddonMasterBase<AtkUnitBase>
{
    protected AddonMasterBase(nint addon) : base(addon)
    {
    }

    protected AddonMasterBase(void* addon) : base(addon)
    {
    }
}

public interface IAddonMasterBase { }