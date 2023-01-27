using Dalamud.Game.Gui.FlyText;
using System;

namespace ECommons.Hooks.ActionEffectTypes
{
    public struct ActionEffectInfo
    {
        public ActionStep step;
        public ulong tick;

        public uint actionId;
        public ActionEffectType type;
        public FlyTextKind kind;
        public uint sourceId;
        public ulong targetId;
        public uint value;

        public bool Equals(ActionEffectInfo other) => step == other.step && tick == other.tick && actionId == other.actionId && type == other.type && kind == other.kind && sourceId == other.sourceId && targetId == other.targetId && value == other.value;
        public override bool Equals(object obj) => obj is ActionEffectInfo other && Equals(other);
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add((int)step);
            hashCode.Add(tick);
            hashCode.Add(actionId);
            hashCode.Add((int)type);
            hashCode.Add((int)kind);
            hashCode.Add(sourceId);
            hashCode.Add(targetId);
            hashCode.Add(value);
            return hashCode.ToHashCode();
        }

        public override string ToString() => $"{nameof(step)}: {step}, {nameof(tick)}: {tick}, {nameof(actionId)}: {actionId}, {nameof(type)}: {type}, {nameof(kind)}: {kind}, {nameof(sourceId)}: {sourceId}, {nameof(targetId)}: {targetId}, {nameof(value)}: {value}";
    }
}
