using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.UIInput;/*
[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe partial struct AtkEvent
{
    [FieldOffset(0x0)] public FFXIVClientStructs.FFXIV.Component.GUI.AtkEvent CSEvent;
    [FieldOffset(0x0)] public AtkResNode* Node; // extra node param, unused a lot
    [FieldOffset(0x8)] public AtkEventTarget* Target; // target of event (eg clicking a button, target is the button node)
    [FieldOffset(0x10)] public AtkEventListener* Listener; // listener of event
    [FieldOffset(0x18)] public uint Param; // arg3 of ReceiveEvent
    [FieldOffset(0x20)] public AtkEvent* NextEvent;
    [FieldOffset(0x28)] public AtkEventType Type; // TODO: Change enum to uint
    [FieldOffset(0x29)] public byte Unk29;
    [FieldOffset(0x2A)] public byte Flags; // 0: handled, 5: force handled (see AtkEvent::SetEventIsHandled)
}
*/