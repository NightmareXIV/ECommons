using ECommons.ExcelServices.TerritoryEnumeration;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Text.RegularExpressions;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe partial class RelicNoteBook : AddonMasterBase<AddonRelicNoteBook>
    {
        public RelicNoteBook(nint addon) : base(addon) { }
        public RelicNoteBook(void* addon) : base(addon) { }

        public override string AddonDescription => "ARR Relic Trial of the Braves book";

        public RelicNoteBookEnemy[] Enemies
        {
            get
            {
                var relicNoteBookEnemy = new RelicNoteBookEnemy[10];
                for(var i = 0; i < relicNoteBookEnemy.Length; i++)
                {
                    relicNoteBookEnemy[i] = new RelicNoteBookEnemy(this, Addon, GetEnemyCheckBox(i), i);
                }
                return relicNoteBookEnemy;
            }
        }

        public RelicNoteBookDungeon[] Dungeons
        {
            get
            {
                var relicNoteBookDungeon = new RelicNoteBookDungeon[3];
                for(var i = 0; i < relicNoteBookDungeon.Length; i++)
                {
                    relicNoteBookDungeon[i] = new RelicNoteBookDungeon(this, Addon, GetDungeonCheckBox(i), i);
                }
                return relicNoteBookDungeon;
            }
        }

        public RelicNoteBookFate[] Fates
        {
            get
            {
                var relicNoteBookFate = new RelicNoteBookFate[3];
                for(var i = 0; i < relicNoteBookFate.Length; i++)
                {
                    relicNoteBookFate[i] = new RelicNoteBookFate(this, Addon, GetFateCheckBox(i), i);
                }
                return relicNoteBookFate;
            }
        }

        public RelicNoteBookLeve[] Leves
        {
            get
            {
                var relicNoteBookEnemy = new RelicNoteBookLeve[10];
                for(var i = 0; i < relicNoteBookEnemy.Length; i++)
                {
                    relicNoteBookEnemy[i] = new RelicNoteBookLeve(this, Addon, GetLeveCheckBox(i), i);
                }
                return relicNoteBookEnemy;
            }
        }

        public class RelicNoteBookEnemy
        {
            private RelicNoteBook addonMaster;
            private AddonRelicNoteBook* addon;
            private AtkComponentCheckBox* checkbox;
            private int index;

            public RelicNoteBookEnemy(RelicNoteBook addonMaster, AddonRelicNoteBook* addon, AtkComponentCheckBox* checkbox, int index)
            {
                this.addonMaster = addonMaster;
                this.addon = addon;
                this.checkbox = checkbox;
                this.index = index;
            }

            public AtkComponentCheckBox* CheckBox => checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public void Click() => addonMaster.ClickCheckboxIfEnabled(CheckBox);
        }

        public class RelicNoteBookDungeon
        {
            private RelicNoteBook addonMaster;
            private AddonRelicNoteBook* addon;
            private AtkComponentCheckBox* checkbox;
            private int index;

            public RelicNoteBookDungeon(RelicNoteBook addonMaster, AddonRelicNoteBook* addon, AtkComponentCheckBox* checkbox, int index)
            {
                this.addonMaster = addonMaster;
                this.addon = addon;
                this.checkbox = checkbox;
                this.index = index;
            }

            public AtkComponentCheckBox* CheckBox => checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public void Click() => addonMaster.ClickCheckboxIfEnabled(CheckBox);
        }

        public class RelicNoteBookFate
        {
            private RelicNoteBook addonMaster;
            private AddonRelicNoteBook* addon;
            private AtkComponentCheckBox* checkbox;
            private int index;

            public RelicNoteBookFate(RelicNoteBook addonMaster, AddonRelicNoteBook* addon, AtkComponentCheckBox* checkbox, int index)
            {
                this.addonMaster = addonMaster;
                this.addon = addon;
                this.checkbox = checkbox;
                this.index = index;
            }

            public AtkComponentCheckBox* CheckBox => checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public void Click() => addonMaster.ClickCheckboxIfEnabled(CheckBox);
        }

        public class RelicNoteBookLeve
        {
            private RelicNoteBook addonMaster;
            private AddonRelicNoteBook* addon;
            private AtkComponentCheckBox* checkbox;
            private int index;

            public RelicNoteBookLeve(RelicNoteBook addonMaster, AddonRelicNoteBook* addon, AtkComponentCheckBox* checkbox, int index)
            {
                this.addonMaster = addonMaster;
                this.addon = addon;
                this.checkbox = checkbox;
                this.index = index;
            }

            public AtkComponentCheckBox* CheckBox => checkbox;
            public bool IsEnabled => CheckBox->IsEnabled;
            public void Click() => addonMaster.ClickCheckboxIfEnabled(CheckBox);
        };

        private AtkComponentCheckBox* GetEnemyCheckBox(int index) => index switch
        {
            0 => Addon->Enemy0.CheckBox,
            1 => Addon->Enemy1.CheckBox,
            2 => Addon->Enemy2.CheckBox,
            3 => Addon->Enemy3.CheckBox,
            4 => Addon->Enemy4.CheckBox,
            5 => Addon->Enemy5.CheckBox,
            6 => Addon->Enemy6.CheckBox,
            7 => Addon->Enemy7.CheckBox,
            8 => Addon->Enemy8.CheckBox,
            9 => Addon->Enemy9.CheckBox,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        private AtkComponentCheckBox* GetDungeonCheckBox(int index) => index switch
        {
            0 => Addon->Dungeon0.CheckBox,
            1 => Addon->Dungeon1.CheckBox,
            2 => Addon->Dungeon2.CheckBox,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        private AtkComponentCheckBox* GetFateCheckBox(int index) => index switch
        {
            0 => Addon->Fate0.CheckBox,
            1 => Addon->Fate1.CheckBox,
            2 => Addon->Fate2.CheckBox,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        private AtkComponentCheckBox* GetLeveCheckBox(int index) => index switch
        {
            0 => Addon->Leve0.CheckBox,
            1 => Addon->Leve1.CheckBox,
            2 => Addon->Leve2.CheckBox,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumber();
    }
}
