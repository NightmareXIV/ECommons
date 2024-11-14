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
    
    /// <summary>
    /// User-friendly description, for use in plugin settings, etc.
    /// </summary>
    public abstract string AddonDescription { get; }
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

    [Obsolete("For the intended functionality please use HasFocus. For the same functionality please use IsAddonInFocusList.")]
    public bool IsAddonFocused => IsAddonInFocusList;
    public bool IsAddonOnlyFocusListEntry => RaptureAtkUnitManager.Instance()->FocusedUnitsList.Count == 1 && RaptureAtkUnitManager.Instance()->FocusedUnitsList.Entries[0].Value == Base;

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
        var ret = stackalloc AtkEvent[]
        {
            new()
            {
                Listener = (AtkEventListener*)Base,
                Target = &AtkStage.Instance()->AtkEventTarget,
                State = new()
                {
                    StateFlags = (AtkEventStateFlags)flags
                }
            } 
        };
        return *ret;
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
    string AddonDescription { get; }
    unsafe AtkUnitBase* Base { get; }
    bool HasFocus { get; }
    bool IsAddonInFocusList { get; }
    bool IsAddonOnlyFocusListEntry { get; }
    bool IsAddonReady { get; }
    bool IsVisible { get; }
}