using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using ECommons.DalamudServices;
using ECommons.EzSharedDataManager;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.EzContextMenu;
/// <summary>
/// A helper class to remove context menu prefix that is required by Dalamud. <br></br>
/// While removing prefix, please still make sure to give an user indication that menu item is added by the plugin.
/// </summary>
public static unsafe class ContextMenuPrefixRemover
{
    private static bool Initialized = false;
    private static SeIconChar DalamudPrefix;
    private static ushort DalamudPrefixColor;

    /// <summary>
    /// Manual initialization is optional. Initialization will be automatically performed upon first call of <see cref="RemovePrefix"/> method.
    /// </summary>
    public static void Initialize()
    {
        try
        {
            if(!Initialized)
            {
                Initialized = true;
                var data = EzSharedData.GetOrCreate<HashSet<(int, ushort)>>("ECommons.ContextMenuPrefixRemover.TakenContextMenuPrefixes", []);
                var iterations = 0;
                do
                {
                    do
                    {
                        iterations++;
                        DalamudPrefix = (SeIconChar)Random.Shared.Next(1, int.MaxValue);
                    }
                    while(Enum.GetValues<SeIconChar>().Contains(DalamudPrefix) && iterations < 100000);
                    DalamudPrefixColor = (ushort)Random.Shared.Next(1, ushort.MaxValue);
                }
                while(data.Contains(((int)DalamudPrefix, DalamudPrefixColor)));
                data.Add(((int)DalamudPrefix, DalamudPrefixColor));
                PluginLog.Debug($"Initialized prefix remover with prefix={(int)DalamudPrefix}, color={DalamudPrefixColor}, iterations={iterations}");
                Svc.AddonLifecycle.RegisterListener(AddonEvent.PreRequestedUpdate, "ContextMenu", OnContextMenuUpdate);
            }
        }
        catch(Exception e)
        {
            e.Log();
        }
    }

    internal static void Dispose()
    {
        Svc.AddonLifecycle.UnregisterListener(AddonEvent.PreRequestedUpdate, "ContextMenu", OnContextMenuUpdate);
    }

    /// <summary>
    /// Call this extension method to remove prefix from menu item.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static MenuItem RemovePrefix(this MenuItem m)
    {
        Initialize();
        m.Prefix = DalamudPrefix;
        m.PrefixColor = DalamudPrefixColor;
        return m;
    }

    private static void OnContextMenuUpdate(AddonEvent type, AddonArgs args)
    {
        var addon = (AddonContextMenu*)args.Addon;
        var numEntries = addon->AtkValues[0].UInt;
        for(var i = 0; i < numEntries; i++)
        {
            var entry = addon->AtkValues[7 + i];
            if(entry.Type.EqualsAny(ValueType.String, ValueType.ManagedString, ValueType.String8))
            {
                var seString = MemoryHelper.ReadSeStringNullTerminated((nint)entry.String.Value);
                if(seString.Payloads.Count < 2) continue;
                for(var x = 1; x < seString.Payloads.Count; x++)
                {
                    {
                        if(seString.Payloads[x - 1] is UIForegroundPayload fg && fg.ColorKey == DalamudPrefixColor && seString.Payloads[x] is TextPayload payload && payload.Text != null && payload.Text.Length >= 1 && payload.Text[0] == (char)DalamudPrefix)
                        {
                            seString.Payloads.RemoveAt(x);
                            seString.Payloads.Add(new TextPayload("\0"));
                            break;
                        }
                    }
                }
                MemoryHelper.WriteSeString((nint)entry.String.Value, seString);
            }
        }
    }
}
