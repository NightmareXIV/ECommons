using Dalamud.Logging;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Hooks
{
    internal static unsafe class ActionEffect
    {
        internal static void ReceiveActionEffect(uint sourceId, Character* sourceCharacter, IntPtr pos, EffectHeader* effectHeader, EffectEntry* effectArray, ulong* effectTail)
        {
            try
            {
                PluginLog.Verbose($"--- source actor: {sourceCharacter->GameObject.ObjectID}, action id {effectHeader->ActionId}, anim id {effectHeader->AnimationId} numTargets: {effectHeader->TargetCount} ---");

                // TODO: Reimplement opcode logging, if it's even useful. Original code follows
                // ushort op = *((ushort*) effectHeader.ToPointer() - 0x7);
                // DebugLog(Effect, $"--- source actor: {sourceId}, action id {id}, anim id {animId}, opcode: {op:X} numTargets: {targetCount} ---");

                var entryCount = effectHeader->TargetCount switch
                {
                    0 => 0,
                    1 => 8,
                    <= 8 => 64,
                    <= 16 => 128,
                    <= 24 => 192,
                    <= 32 => 256,
                    _ => 0
                };

                for (int i = 0; i < entryCount; i++)
                {
                    if (effectArray[i].type == ActionEffectType.Nothing) continue;

                    var target = effectTail[i / 8];
                    uint dmg = effectArray[i].value;
                    if (effectArray[i].mult != 0)
                        dmg += ((uint)ushort.MaxValue + 1) * effectArray[i].mult;

                    var newEffect = new ActionEffectInfo
                    {
                        step = ActionStep.Effect,
                        actionId = effectHeader->ActionId,
                        type = effectArray[i].type,
                        sourceId = sourceId,
                        targetId = target,
                        value = dmg,
                    };

                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "An error has occurred in Damage Info.");
            }

            _receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTail);
        }
    }
}
