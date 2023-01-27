using Dalamud.Hooking;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Hooks
{
    public static unsafe class ActionEffect
    {
        const string Sig = "4C 89 44 24 ?? 55 56 41 54 41 55 41 56";

        public delegate void ProcessActionEffect(uint sourceId, Character* sourceCharacter, nint pos, EffectHeader* effectHeader, EffectEntry* effectArray, ulong* effectTail);
        internal static Hook<ProcessActionEffect> ProcessActionEffectHook = null;
        static Action<uint, ushort, ActionEffectType, uint, ulong, uint> Callback = null;

        static bool doLogging = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullParamsCallback">uint ActionID, ushort animationID, ActionEffectType type, uint sourceID, ulong targetOID, uint damage</param>
        /// <param name="logging"></param>
        /// <exception cref="Exception"></exception>
        public static void Init(Action<uint, ushort, ActionEffectType, uint, ulong, uint> fullParamsCallback, bool logging = false)
        {
            if (ProcessActionEffectHook != null)
            {
                throw new Exception("Action Effect Hook is already initialized!");
            }
            if (Svc.SigScanner.TryScanText(Sig, out var ptr))
            {
                Callback = fullParamsCallback;
                ProcessActionEffectHook = Hook<ProcessActionEffect>.FromAddress(ptr, ProcessActionEffectDetour);
                Enable();
                PluginLog.Information($"Requested Action Effect hook and successfully initialized");
            }
            else
            {
                PluginLog.Error($"Could not find ActionEffect signature");
            }
        }

        public static void Enable()
        {
            if (ProcessActionEffectHook?.IsEnabled == false) ProcessActionEffectHook?.Enable();
        }

        public static void Disable()
        {
            if (ProcessActionEffectHook?.IsEnabled == true) ProcessActionEffectHook?.Disable();
        }

        internal static void Dispose()
        {
            if (ProcessActionEffectHook != null)
            {
                PluginLog.Information($"Disposing Action Effect Hook");
                Disable();
                ProcessActionEffectHook?.Dispose();
                ProcessActionEffectHook = null;
            }
        }

        internal static void ProcessActionEffectDetour(uint sourceID, Character* sourceCharacter, IntPtr pos, EffectHeader* effectHeader, EffectEntry* effectArray, ulong* effectTail)
        {
            try
            {
                if(doLogging) PluginLog.Verbose($"--- source actor: {sourceCharacter->GameObject.ObjectID}, action id {effectHeader->ActionID}, anim id {effectHeader->AnimationId} numTargets: {effectHeader->TargetCount} ---");

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

                    var targetID = effectTail[i / 8];
                    uint dmg = effectArray[i].value;
                    if (effectArray[i].mult != 0)
                        dmg += ((uint)ushort.MaxValue + 1) * effectArray[i].mult;

                    /*var newEffect = new ActionEffectInfo
                    {
                        actionId = effectHeader->ActionID,
                        type = effectArray[i].type,
                        sourceId = sourceID,
                        targetId = targetID,
                        value = dmg,
                    };*/

                    Callback(effectHeader->ActionID, effectHeader->AnimationId, effectArray[i].type, sourceID, targetID, dmg);

                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "An error has occurred in Action Effect hook.");
            }

            ProcessActionEffectHook.Original(sourceID, sourceCharacter, pos, effectHeader, effectArray, effectTail);
        }
    }
}
