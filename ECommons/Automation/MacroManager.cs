using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.Automation;
#nullable disable

public static unsafe class MacroManager
{
    public static void Execute(string multilineString)
    {
        Execute(multilineString.Replace("\r", "").Split("\n"));
    }

    public static void Execute(params string[] commands)
    {
        Execute((IEnumerable<string>)commands);
    }

    public static void Execute(IEnumerable<string> commands)
    {
        GenericHelpers.Safe(delegate
        {
            if(FromLines(commands) is RaptureMacroModule.Macro macro)
                RaptureShellModule.Instance()->ExecuteMacro(&macro);
        });
    }

    public static RaptureMacroModule.Macro? FromLines(IEnumerable<string> commands)
        => FromLines(commands.Select(x => (SeString)x));

    public static RaptureMacroModule.Macro? FromLines(IEnumerable<SeString> commands)
    {
        if(commands.Count() > 15)
        {
            throw new InvalidOperationException("Macro was more than 15 lines!");
        }
        if(commands.Any(x => x.Encode().Length > 180))
        {
            throw new InvalidOperationException("Macro contained lines more than 180 symbols!");
        }
        if(commands.Any(x => x.TextValue is var str && (str.Contains('\n') || str.Contains('\r') || str.Contains('\0') || Chat.SanitiseText(str).Length != str.Length)))
        {
            throw new InvalidOperationException("Macro contained invalid symbols!");
        }

        var macro = new RaptureMacroModule.Macro();
        macro.Name.Ctor();
        foreach(ref var line in macro.Lines)
            line.Ctor();

        try
        {
            foreach(var (line, index) in commands.WithIndex())
            {
                if(line.Payloads.Count == 0 || line.Encode().Length == 0)
                {
                    macro.Lines[index].Clear();
                    continue;
                }

                var encoded = line.Encode();
                if(encoded.Length == 0 || encoded.Any(c => c == 0))
                {
                    macro.Lines[index].Clear();
                    continue;
                }

                fixed(byte* encodedPtr = encoded)
                {
                    macro.Lines[index].SetString(encodedPtr);
                }
            }

            return macro;
        }
        catch(Exception e)
        {
            Svc.Log.Error(e, $"Failed to create macro from lines");
        }

        return null;
    }
}
