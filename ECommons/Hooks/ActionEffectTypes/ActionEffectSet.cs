using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Numerics;
using Action = Lumina.Excel.GeneratedSheets.Action;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace ECommons.Hooks.ActionEffectTypes;

public unsafe struct ActionEffectSet
{
    public Action Action { get; }

    public GameObject Target { get; }

    public GameObject Source { get; }

    public Character SourceCharacter { get; }

    public TargetEffect[] TargetEffects { get; }

    public Vector3 Position { get; }

    public EffectHeader Header { get; }

    public ActionEffectSet(uint sourceID, Character* sourceCharacter, Vector3* pos, EffectHeader* effectHeader, EffectEntry* effectArray, ulong* effectTail)
    {
        Action = Svc.Data.GetExcelSheet<Action>().GetRow(effectHeader->ActionID);
        Target = Svc.Objects.SearchById(effectHeader->AnimationTargetId);
        Source = Svc.Objects.SearchById(sourceID);
        SourceCharacter = *sourceCharacter;
        Position = *pos;
        Header = *effectHeader;

        TargetEffects = new TargetEffect[effectHeader->TargetCount];
        for (int i = 0; i < effectHeader->TargetCount; i++)
        {
            TargetEffects[i] = new TargetEffect(effectTail[i], effectArray + 8 * i);
        }
    }

    public Dictionary<ulong, uint> GetSpecificTypeEffect(ActionEffectType type)
    {
        var result = new Dictionary<ulong, uint>();
        foreach (var effect in TargetEffects)
        {
            if (effect.GetSpecificTypeEffect(type, out var e))
            {
                //Is this value or Damage? IDK about it.
                result[effect.TargetID] = e.value;
            }
        }
        return result;
    }

    public override string ToString()
    {
        var str = $"S:{Source?.Name}, T:{Target?.Name}, Lock:{Header.AnimationLockTime}";
        str += $"\nType: {Header.ActionType}, Name: {Action?.Name}({Action?.RowId})";
        if (TargetEffects != null)
        {
            foreach (var effect in TargetEffects)
            {
                str += "\n" + effect.ToString();
            }
        }
        return str;
    }
}
