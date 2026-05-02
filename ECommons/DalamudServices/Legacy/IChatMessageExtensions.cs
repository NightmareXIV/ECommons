using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.DalamudServices.Legacy;

public static class IChatMessageExtensions
{
    extension(IChatMessage value)
    {
        public XivChatType Type
        {
            get
            {
                var ret = new LogInfo()
                {
                    LogKind = (ushort)value.LogKind,
                    SourceKind = (EntityRelationKind)value.SourceKind,
                    TargetKind = (EntityRelationKind)value.TargetKind,
                };
                return (XivChatType)ret.Value;
            }
        }
    }
}
