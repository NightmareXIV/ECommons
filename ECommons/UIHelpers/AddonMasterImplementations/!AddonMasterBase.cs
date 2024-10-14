using ECommons.Automation.UIInput;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

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

    public bool HasFocus
    {
        get
        {
            var focus = AtkStage.Instance()->GetFocus();
            if (focus == null) return false;
            for (var i = 0; i < RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count; i++)
            {
                var atk = RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[i].Value;
                if (atk != null && atk->RootNode == GenericHelpers.GetRootNode(focus))
                    return true;
            }
            return false;
        }
    }

    public bool IsAddonInFocusList
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

    [Obsolete("Please use IsAddonInFocusList instead.")]
    public bool IsAddonFocused => IsAddonInFocusList;
    public bool IsAddonHighestFocus => RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count - 1].Value == Base;
    public bool IsAddonOnlyFocus => RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count == 1 && RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[0].Value == Base;

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

public unsafe interface IAddonMasterBase
{
    unsafe AtkUnitBase* Base { get; }
    bool IsAddonFocused { get; }
    bool IsAddonHighestFocus { get; }
    bool IsAddonOnlyFocus { get; }
    bool IsAddonReady { get; }
    bool IsVisible { get; }
}