using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECommons.GameFunctions;

public static unsafe class CharacterFunctions
{

    public static ushort GetVFXId(void* VfxData)
    {
        return VfxData == null ? (ushort)0 : *(ushort*)((IntPtr)VfxData + 8);
    }


    extension(IBattleChara chr)
    {
        public float RemainingCastTime => chr.CastInfo.TotalCastTime - chr.CastInfo.CurrentCastTime;
        public CastInfo CastInfo
        {
            get
            {
                var info = chr.Struct()->GetCastInfo();
                if(info == null)
                {
                    return default;
                }
                var ret = *info;
                //ret.ActionType = (*(byte*)(((nint)info) + 1));
                return ret;
            }
        }

        public BattleChara* Struct()
        {
            return (BattleChara*)chr.Address;
        }

        public Character* Character()
        {
            return (Character*)chr.Address;
        }

        public GameObject* GameObject()
        {
            return (GameObject*)chr.Address;
        }

        public bool HasStatus(uint id, float? lessThan = null, float? moreThan = null)
        {
            foreach(var x in chr.StatusList)
            {
                if(x.StatusId == id)
                {
                    if(lessThan != null && x.RemainingTime > lessThan.Value)
                    {
                        continue;
                    }

                    if(moreThan != null && x.RemainingTime < moreThan.Value)
                    {
                        continue;
                    }

                    return true;
                }
            }
            return false;
        }

        public bool HasStatus(uint id, out float time, float? lessThan = null, float? moreThan = null)
        {
            foreach(var x in chr.StatusList)
            {
                if(x.StatusId == id)
                {
                    if(lessThan != null && x.RemainingTime > lessThan.Value)
                    {
                        continue;
                    }

                    if(moreThan != null && x.RemainingTime < moreThan.Value)
                    {
                        continue;
                    }

                    time = x.RemainingTime;
                    return true;
                }
            }
            time = default;
            return false;
        }

        public bool HasStatus(IEnumerable<uint> id, float? lessThan = null, float? moreThan = null)
        {
            foreach(var x in id)
            {
                if(chr.HasStatus(x, lessThan, moreThan))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasStatus(IEnumerable<uint> id, out List<(uint ID, float Time)> foundStatus, float? lessThan = null, float? moreThan = null)
        {
            foundStatus = [];
            foreach(var x in id)
            {
                if(chr.HasStatus(x, out var time, lessThan, moreThan))
                {
                    foundStatus.Add((x, time));
                }
            }
            return foundStatus.Count > 0;
        }

        public bool IsCasting(uint spellId = 0, ActionType? type = null)
        {
            var info = chr.CastInfo;
            return info.ActionId == 0
                ? false
                : chr.IsCasting && (spellId == 0 || (info.ActionId.EqualsAny(spellId) && (type == null || info.ActionType == (byte)type.Value)));
        }

        public bool IsCasting(params uint[] spellId)
        {
            var info = chr.CastInfo;
            return info.ActionId == 0 ? false : chr.IsCasting && info.ActionId.EqualsAny(spellId);
        }

        public bool IsCasting(IEnumerable<uint> spellId)
        {
            var info = chr.CastInfo;
            return info.ActionId == 0 ? false : chr.IsCasting && info.ActionId.EqualsAny(spellId);
        }
    }

    extension(IGameObject obj)
    {
        public uint ObjectId => obj.EntityId;
        public Vector2 Position2 => obj.Position.ToVector2();
    }

    extension(ICharacter chr)
    {
        public float Health => (float)chr.CurrentHp / (float)chr.MaxHp;
        public uint MissingHp => chr.MaxHp - chr.CurrentHp;
        public uint StatusLoop => chr.Struct()->StatusLoopVfxId;
        public int ModelId => chr.Struct()->ModelContainer.ModelCharaId;

        public Character* Struct()
        {
            return (Character*)chr.Address;
        }

        public GameObject* IGameObject()
        {
            return (GameObject*)chr.Address;
        }

        public bool IsCharacterVisible()
        {
            var v = (IntPtr)((Character*)chr.Address)->GameObject.DrawObject;
            return v == IntPtr.Zero ? false : Bitmask.IsBitSet(*(byte*)(v + 136), 0);
        }

        public byte GetTransformationID()
        {
            return chr.Struct()->Timeline.ModelState;
            //return *(byte*)(chr.Address + 2480 + 704);
        }

        public bool IsInWater()
        {
            return *(byte*)(chr.Address + 1452) == 1;
        }

        public CombatRole GetRole()
        {
            if(chr.ClassJob.ValueNullable?.Role == 1)
            {
                return CombatRole.Tank;
            }

            if(chr.ClassJob.ValueNullable?.Role == 2)
            {
                return CombatRole.DPS;
            }

            if(chr.ClassJob.ValueNullable?.Role == 3)
            {
                return CombatRole.DPS;
            }

            return chr.ClassJob.ValueNullable?.Role == 4 ? CombatRole.Healer : CombatRole.NonCombat;
        }
        public List<TetherInfo> GetTethers(bool onlySource = false)
        {
            List<TetherInfo> ret = [];
            {
                var t = chr.Struct()->Vfx.Tethers;
                for(var i = 0; i < t.Length; i++)
                {
                    if(t[i].Id != 0)
                    {
                        ret.Add(new(t[i], t[i].TargetId.ObjectId, true));
                    }
                }
            }
            if(!onlySource)
            {
                foreach(var obj in Svc.Objects)
                {
                    if(obj is ICharacter targetChara)
                    {
                        var t = targetChara.Struct()->Vfx.Tethers;
                        for(var i = 0; i < t.Length; i++)
                        {
                            if(t[i].Id != 0 && t[i].TargetId == chr.GameObjectId)
                            {
                                ret.Add(new(t[i], targetChara.ObjectId, false));
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}
