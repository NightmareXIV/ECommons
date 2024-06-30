using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.UIInput;
public unsafe class ClickHelper
{
    private static InvokeListener? ListenerInternal;
    public delegate nint InvokeListener(nint a1, AtkEventType a2, uint a3, AtkEvent* a4);
    public static InvokeListener Listener 
    {
        get
        {
            ListenerInternal ??= EzDelegate.Get<InvokeListener>("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 0F B7 FA");
            return ListenerInternal;
        }
    }

    public unsafe delegate IntPtr ReceiveEventDelegate(AtkEventListener* eventListener, EventType evt, uint which, void* eventData, void* inputData);

    public static ReceiveEventDelegate GetReceiveEvent(AtkEventListener* listener)
    {
        var receiveEventAddress = new IntPtr(listener->VirtualTable->ReceiveEvent);
        return Marshal.GetDelegateForFunctionPointer<ReceiveEventDelegate>(receiveEventAddress)!;
    }

    public static void InvokeReceiveEvent(AtkEventListener* eventListener, EventType type, uint which, EventData eventData, InputData inputData)
    {
        var receiveEvent = GetReceiveEvent(eventListener);
        receiveEvent(eventListener, type, which, eventData.Data, inputData.Data);
    }

    public static void ClickAddonComponent(AtkComponentBase* unitbase, AtkComponentNode* target, uint which, EventType type, EventData? eventData = null, InputData? inputData = null)
    {
        EventData? newEventData = null;
        InputData? newInputData = null;
        if(eventData == null)
        {
            newEventData = EventData.ForNormalTarget(target, unitbase);
        }
        if (inputData == null)
        {
            newInputData = InputData.Empty();
        }

        InvokeReceiveEvent(&unitbase->AtkEventListener, type, which, eventData ?? newEventData!, inputData ?? newInputData!);
        newEventData?.Dispose();
        newInputData?.Dispose();
    }
}

public static unsafe class ClickHelperExtensions
{
    public static void ClickAddonButton(this AtkComponentButton target, AtkComponentBase* addon, uint which, EventType type = EventType.CHANGE, EventData? eventData = null)
        => ClickHelper.ClickAddonComponent(addon, target.AtkComponentBase.OwnerNode, which, type, eventData);

    public static void ClickRadioButton(this AtkComponentRadioButton target, AtkComponentBase* addon, uint which, EventType type = EventType.CHANGE)
        => ClickHelper.ClickAddonComponent(addon, target.OwnerNode, which, type);

    public static void ClickAddonButton(this AtkComponentButton target, AtkUnitBase* addon, AtkEvent* eventData)
    {
        ClickHelper.Listener.Invoke((nint)addon, eventData->Type, eventData->Param, eventData);
    }

    public static void ClickAddonButton(this AtkCollisionNode target, AtkUnitBase* addon, AtkEvent* eventData)
    {
        ClickHelper.Listener.Invoke((nint)addon, eventData->Type, eventData->Param, eventData);
    }

    public static void ClickAddonButton(this AtkComponentButton target, AtkUnitBase* addon)
    {
        var btnRes = target.AtkComponentBase.OwnerNode->AtkResNode;
        var evt = btnRes.AtkEventManager.Event;

        addon->ReceiveEvent(evt->Type, (int)evt->Param, btnRes.AtkEventManager.Event);

    }
    public static void ClickAddonButton(this AtkCollisionNode target, AtkUnitBase* addon)
    {
        var btnRes = target.AtkResNode;
        var evt = btnRes.AtkEventManager.Event;

        while (evt->Type != AtkEventType.MouseClick)
            evt = evt->NextEvent;

        addon->ReceiveEvent(evt->Type, (int)evt->Param, btnRes.AtkEventManager.Event);
    }


    public static void ClickRadioButton(this AtkComponentRadioButton target, AtkUnitBase* addon)
    {
        var btnRes = target.OwnerNode->AtkResNode;
        var evt = btnRes.AtkEventManager.Event;

        Svc.Log.Debug($"{evt->Type} {evt->Param}");
        addon->ReceiveEvent(evt->Type, (int)evt->Param, btnRes.AtkEventManager.Event);
    }
}
