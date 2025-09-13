using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace ECommons.GameFunctions;

[Obsolete("Use ExtendedPronoun instead of FakePronoun")]
public static unsafe class FakePronoun
{
    public static GameObject* Resolve(string pronoun) => ExtendedPronoun.Resolve(pronoun);
}

public static unsafe class ExtendedPronoun
{
    public static class Extensions
    {
        private static delegate*<string, string, string, GameObject*>[] _handlers = new delegate*<string, string, string, GameObject*>[16];
        private static int _count = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(delegate*<string, string, string, GameObject*> function)
        {
            if(function == null) return;

            for(int i = 0; i < _count; i++)
            {
                if((IntPtr)_handlers[i] == (IntPtr)function) return; 
            }

            if(_count >= _handlers.Length)
            {
                var newArray = new delegate*<string, string, string, GameObject*>[_handlers.Length * 2];
                for(int i = 0; i < _handlers.Length; i++)
                {
                    newArray[i] = _handlers[i];
                }
                _handlers = newArray;
            }

            _handlers[_count++] = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(delegate*<string, string, string, GameObject*> extension)
        {
            if(extension == null) return;

            for(int i = 0; i < _count; i++)
            {
                if((IntPtr)_handlers[i] == (IntPtr)extension)
                {
                    _count--;
                    _handlers[i] = _handlers[_count];
                    _handlers[_count] = null;
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject* Invoke(string pronoun, string extraCondition, string modifier)
        {
            for(int i = 0; i < _count; i++)
            {
                var ret = _handlers[i](pronoun, extraCondition, modifier);
                if(ret != null) return ret;
            }
            return null;
        }
    }

    public static GameObject* Resolve(string pronoun)
    {
        try
        {
            if(pronoun.Length < 2) return null;
            var stripped = pronoun[1..^1];
            if(Svc.Condition[ConditionFlag.DutyRecorderPlayback])
            {
                if(uint.TryParse(stripped, out var pos))
                {
                    var i = 0;
                    foreach(var x in Svc.Objects)
                    {
                        if(x is IPlayerCharacter pc)
                        {
                            i++;
                            if(i == pos)
                            {
                                return (GameObject*)pc.Address;
                            }
                        }
                    }
                }
                var cand = ResolveInternal(stripped);
                if(cand != null) return cand;
                return null;
            }
            else
            {
                var cand = ResolveInternal(stripped);
                if(cand != null) return cand;
                return FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUIModule()->GetPronounModule()->ResolvePlaceholder($"{pronoun}", 0, 0);
            }
        }
        catch(Exception e)
        {
            e.Log();
            return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static GameObject* ResolveInternal(string key)
    {
        var array = key.Split(':');
        var extraCondition = array.Length > 1 ? array[1] : null;
        var modifier = array.Length > 2 ? array[2] : null;
        return array[0] switch
        {
            "d1" or "m1" => Player(CombatRole.DPS, 1, extraCondition, modifier),
            "d2" or "m2" => Player(CombatRole.DPS, 2, extraCondition, modifier),
            "d3" or "r1" => Player(CombatRole.DPS, 3, extraCondition, modifier),
            "d4" or "r2" => Player(CombatRole.DPS, 4, extraCondition, modifier),
            "d5" => Player(CombatRole.DPS, 5, extraCondition, modifier),
            "d6" => Player(CombatRole.DPS, 6, extraCondition, modifier),
            "d7" => Player(CombatRole.DPS, 7, extraCondition, modifier),
            "d8" => Player(CombatRole.DPS, 8, extraCondition, modifier),
            "t1" or "mt" => Player(CombatRole.Tank, 1, extraCondition, modifier),
            "t2" or "ot" => Player(CombatRole.Tank, 2, extraCondition, modifier),
            "t3" => Player(CombatRole.Tank, 3, extraCondition, modifier),
            "t4" => Player(CombatRole.Tank, 4, extraCondition, modifier),
            "t5" => Player(CombatRole.Tank, 5, extraCondition, modifier),
            "t6" => Player(CombatRole.Tank, 6, extraCondition, modifier),
            "t7" => Player(CombatRole.Tank, 7, extraCondition, modifier),
            "t8" => Player(CombatRole.Tank, 8, extraCondition, modifier),
            "h1" => Player(CombatRole.Healer, 1, extraCondition, modifier),
            "h2" => Player(CombatRole.Healer, 2, extraCondition, modifier),
            "h3" => Player(CombatRole.Healer, 3, extraCondition, modifier),
            "h4" => Player(CombatRole.Healer, 4, extraCondition, modifier),
            "h5" => Player(CombatRole.Healer, 5, extraCondition, modifier),
            "h6" => Player(CombatRole.Healer, 6, extraCondition, modifier),
            "h7" => Player(CombatRole.Healer, 7, extraCondition, modifier),
            "h8" => Player(CombatRole.Healer, 8, extraCondition, modifier),
            "me" => Me(extraCondition, modifier),
            "target" => Target(extraCondition, modifier),
            _ => null
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static GameObject* Player(CombatRole role, int number, string extraCondition, string modifier)
    {
        var i = 0;
        foreach(var x in Svc.Objects)
        {
            if(x is IPlayerCharacter pc && pc.GetRole() == role)
            {
                i++;
                if(i == number)
                {
                    if(modifier == "target")
                    {
                        return (GameObject*)pc.TargetObject?.Address;
                    }
                    return (GameObject*)pc.Address;
                }
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static GameObject* Target(string extraCondition, string modifier)
    {
        if(modifier == "target")
        {
            return (GameObject*)Svc.Targets.Target?.TargetObject?.Address;
        }
        return (GameObject*)Svc.Targets.Target?.Address;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static GameObject* Me(string extraCondition, string modifier)
    {
        if(modifier == "target")
        {
            return (GameObject*)Svc.ClientState.LocalPlayer?.TargetObject?.Address;
        }
        return (GameObject*)Svc.ClientState.LocalPlayer?.Address;
    }
}
