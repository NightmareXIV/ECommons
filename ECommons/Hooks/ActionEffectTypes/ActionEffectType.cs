//This file is authored by lmcintyre and distributed under GNU GPL v3 license. https://github.com/lmcintyre/

namespace ECommons.Hooks.ActionEffectTypes;

public enum ActionEffectType : byte
{
    Nothing = 0,
    Miss = 1,
    FullResist = 2,
    Damage = 3,
    Heal = 4,
    BlockedDamage = 5,
    ParriedDamage = 6,
    Invulnerable = 7,
    NoEffectText = 8,
    Unknown_0 = 9,
    MpLoss = 10,
    MpGain = 11,
    TpLoss = 12,
    TpGain = 13,
    GpGain = 14,
    ApplyStatusEffectTarget = 15,
    ApplyStatusEffectSource = 16,
    StatusNoEffect = 20,
    Unknown0 = 27,
    Unknown1 = 28,
    Knockback = 33,
    Mount = 40,
    VFX = 59,
    JobGauge = 61,
};
