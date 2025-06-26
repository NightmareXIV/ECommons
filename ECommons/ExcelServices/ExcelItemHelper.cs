using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace ECommons.ExcelServices;

public static class ExcelItemHelper
{
    /// <summary>
    /// Gets <see cref="Item"/> by id or null if not found.
    /// </summary>
    /// <param name="rowId"></param>
    /// <returns></returns>
    public static Item? Get(int rowId) => Get((uint)rowId);

    /// <summary>
    /// Gets <see cref="Item"/> by id or null if not found.
    /// </summary>
    /// <param name="rowId"></param>
    /// <returns></returns>
    public static Item? Get(uint rowId) => Svc.Data.GetExcelSheet<Item>()!.GetRowOrDefault(rowId);

    /// <summary>
    /// Gets item name. If name or item is missing, prints item's ID. Item names are stripped off non-text payloads. Results are cached.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeID">Force include ID into text</param>
    /// <returns></returns>
    public static string GetName(uint id, bool includeID = false)
    {
        if(ItemNameCache.TryGetValue(id, out var ret)) return ret;
        var data = Svc.Data.GetExcelSheet<Item>()!.GetRowOrDefault(id);
        if(data == null) return $"#{id}";
        return GetName(data);
    }

    private static Dictionary<uint, string> ItemNameCache = [];
    /// <summary>
    /// Gets item name. If name is missing, prints item's ID. Item names are stripped off non-text payloads. Results are cached.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="includeID"></param>
    /// <returns></returns>
    public static string GetName(this Item? item, bool includeID = false)
    {
        if(item == null) return "? Unknown ?";
        if(!ItemNameCache.TryGetValue(item.Value.RowId, out var name))
        {
            name = item.Value.Name.GetText();
            ItemNameCache[item.Value.RowId] = name;
        }
        if(name == "")
        {
            return $"#{item.Value.RowId}";
        }
        return name;
    }
    public static string GetName(this Item item, bool includeID = false) => GetName((Item?)item, includeID);

    public static int GetStat(this Item item, BaseParamEnum param, bool isHq = false)
    {
        var ret = 0;
        for(var i = 0; i < item.BaseParam.Count; i++)
        {
            if(item.BaseParam[i].RowId == (int)param)
            {
                ret += item.BaseParamValue[i];
            }
        }
        if(isHq)
        {
            for(var i = 0; i < item.BaseParamSpecial.Count; i++)
            {
                if(item.BaseParamSpecial[i].RowId == (int)param)
                {
                    ret += item.BaseParamValueSpecial[i];
                }
            }
        }
        return ret;
    }
    public static int GetStatCap(this InventoryItem item, BaseParamEnum baseParam)
    {
        if(ExcelItemHelper.Get(item.GetItemId() % 1000000).TryGetValue(out var data))
        {
            return data.GetStatCap(baseParam);
        }
        return -1;
    }

    public static int GetStatCap(this Item item, BaseParamEnum baseParam)
    {
        var level = item.LevelItem.Value;
        var baseValue = baseParam switch
        {
            BaseParamEnum.Strength => level.Strength,
            BaseParamEnum.Dexterity => level.Dexterity,
            BaseParamEnum.Vitality => level.Vitality,
            BaseParamEnum.Intelligence => level.Intelligence,
            BaseParamEnum.Mind => level.Mind,
            BaseParamEnum.Piety => level.Piety,
            BaseParamEnum.HP => level.HP,
            BaseParamEnum.MP => level.MP,
            BaseParamEnum.TP => level.TP,
            BaseParamEnum.GP => level.GP,
            BaseParamEnum.CP => level.CP,
            BaseParamEnum.Delay => level.Delay,
            BaseParamEnum.Tenacity => level.Tenacity,
            BaseParamEnum.AttackPower => level.AttackPower,
            BaseParamEnum.Defense => level.Defense,
            BaseParamEnum.DirectHitRate => level.DirectHitRate,
            BaseParamEnum.Evasion => level.Evasion,
            BaseParamEnum.MagicDefense => level.MagicDefense,
            BaseParamEnum.CriticalHit => level.CriticalHit,
            BaseParamEnum.AttackMagicPotency => level.AttackMagicPotency,
            BaseParamEnum.HealingMagicPotency => level.HealingMagicPotency,
            BaseParamEnum.Determination => level.Determination,
            BaseParamEnum.SkillSpeed => level.SkillSpeed,
            BaseParamEnum.SpellSpeed => level.SpellSpeed,
            BaseParamEnum.Haste => level.Haste,
            BaseParamEnum.Craftsmanship => level.Craftsmanship,
            BaseParamEnum.Control => level.Control,
            BaseParamEnum.Gathering => level.Gathering,
            BaseParamEnum.Perception => level.Perception,
            _ => -1
        };
        if(baseValue != -1 && Svc.Data.GetExcelSheet<BaseParam>().TryGetRow((uint)baseParam, out var row))
        {
            var mult = ((EquipSlotCategoryEnum)item.EquipSlotCategory.RowId) switch
            {
                EquipSlotCategoryEnum.WeaponMainHand => row.OneHandWeaponPercent,
                EquipSlotCategoryEnum.OffHand => row.OffHandPercent,
                EquipSlotCategoryEnum.Head => row.HeadPercent,
                EquipSlotCategoryEnum.Body => row.ChestPercent,
                EquipSlotCategoryEnum.Gloves => row.HandsPercent,
                EquipSlotCategoryEnum.Waist => row.WaistPercent,
                EquipSlotCategoryEnum.Legs => row.LegsPercent,
                EquipSlotCategoryEnum.Feet => row.FeetPercent,
                EquipSlotCategoryEnum.Ears => row.EarringPercent,
                EquipSlotCategoryEnum.Neck => row.NecklacePercent,
                EquipSlotCategoryEnum.Wrists => row.BraceletPercent,
                EquipSlotCategoryEnum.Ring => row.RingPercent,
                EquipSlotCategoryEnum.WeaponTwoHand => row.TwoHandWeaponPercent,
                EquipSlotCategoryEnum.BodyHead => row.ChestHeadPercent,
                EquipSlotCategoryEnum.LegsFeet => row.LegsFeetPercent,
                EquipSlotCategoryEnum.BodyHeadGlovesLegsFeet => row.HeadChestHandsLegsFeetPercent,
                EquipSlotCategoryEnum.BodyLegsFeet => row.ChestLegsFeetPercent,
                _ => -1
            };
            if(mult != -1)
            {
                return (int)((float)baseValue * (float)mult / 1000f);
            }
        }
        return -1;
    }

    public static int GetStat(this InventoryItem item, BaseParamEnum param)
    {
        var ret = 0;
        if(ExcelItemHelper.Get(item.GetItemId() % 1000000).TryGetValue(out var data))
        {
            ret += GetStat(data, param, item.GetItemId() > 1000000);
        }
        for(byte i = 0; i < item.GetMateriaCount(); i++)
        {
            var m = item.GetMateriaId(i);
            var grade = item.GetMateriaGrade(i);
            if(Svc.Data.GetExcelSheet<Materia>().TryGetRow(m, out var mData))
            {
                if(mData.BaseParam.RowId == (int)param)
                {
                    ret += mData.Value[grade];
                }
            }
        }
        var cap = item.GetStatCap(param);
        if(cap > 0 && ret > cap) return cap;
        return ret;
    }

    public static ItemRarity GetRarity(this Item item)
    {
        return (ItemRarity)item.Rarity;
    }
}
