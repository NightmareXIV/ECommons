using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.CSExtensions;

public unsafe static class EventObjectExtensions
{
    extension(ref EventObject thisRef)
    {
        public ushort AnimationId
        {
            get
            {
                fixed(EventObject* ptr = &thisRef)
                {
                    return *(ushort*)(((nint)ptr) + sizeof(GameObject) + 18);
                }
            }
            set
            {
                fixed(EventObject* ptr = &thisRef)
                {
                    *(ushort*)(((nint)ptr) + sizeof(GameObject) + 18) = value;
                }
            }
        }
    }

    extension(IEventObj thisRef)
    {
        public ushort AnimationId
        {
            get => thisRef.Struct()->AnimationId;
            set => thisRef.Struct()->AnimationId = value;
        }
    }
}
