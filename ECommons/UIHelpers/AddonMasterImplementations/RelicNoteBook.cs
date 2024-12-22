using ECommons.ExcelServices.TerritoryEnumeration;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System.Net;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RelicNoteBook : AddonMasterBase<AddonRelicNoteBook>
    {
        public RelicNoteBook(nint addon) : base(addon) { }
        public RelicNoteBook(void* addon) : base(addon) { }

        public override string AddonDescription => "ARR Relic Trial of the Braves book";

        // Enemies
        public AtkComponentCheckBox* Enemy0 => Addon->Enemy0.CheckBox;
        public AtkComponentCheckBox* Enemy1 => Addon->Enemy1.CheckBox;
        public AtkComponentCheckBox* Enemy2 => Addon->Enemy2.CheckBox;
        public AtkComponentCheckBox* Enemy3 => Addon->Enemy3.CheckBox;
        public AtkComponentCheckBox* Enemy4 => Addon->Enemy4.CheckBox;
        public AtkComponentCheckBox* Enemy5 => Addon->Enemy5.CheckBox;
        public AtkComponentCheckBox* Enemy6 => Addon->Enemy6.CheckBox;
        public AtkComponentCheckBox* Enemy7 => Addon->Enemy7.CheckBox;
        public AtkComponentCheckBox* Enemy8 => Addon->Enemy8.CheckBox;
        public AtkComponentCheckBox* Enemy9 => Addon->Enemy9.CheckBox;

        // Dungeons
        public AtkComponentCheckBox* Dungeon0 => Addon->Dungeon0.CheckBox;
        public AtkComponentCheckBox* Dungeon1 => Addon->Dungeon1.CheckBox;
        public AtkComponentCheckBox* Dungeon2 => Addon->Dungeon2.CheckBox;

        // FATEs
        public AtkComponentCheckBox* Fate0 => Addon->Fate0.CheckBox;
        public AtkComponentCheckBox* Fate1 => Addon->Fate1.CheckBox;
        public AtkComponentCheckBox* Fate2 => Addon->Fate2.CheckBox;

        // Leves
        public AtkComponentCheckBox* Leve0 => Addon->Leve0.CheckBox;
        public AtkComponentCheckBox* Leve1 => Addon->Leve1.CheckBox;
        public AtkComponentCheckBox* Leve2 => Addon->Leve2.CheckBox;

        //public void ClickEnemy0() => ClickButtonIfEnabled(&Enemy0->AtkComponentButton);
        public void ClickEnemy0() => ClickCheckboxIfEnabled(Enemy0);
        public void ClickEnemy1() => ClickCheckboxIfEnabled(Enemy1);
        public void ClickEnemy2() => ClickCheckboxIfEnabled(Enemy2);
        public void ClickEnemy3() => ClickCheckboxIfEnabled(Enemy3);
        public void ClickEnemy4() => ClickCheckboxIfEnabled(Enemy4);
        public void ClickEnemy5() => ClickCheckboxIfEnabled(Enemy5);
        public void ClickEnemy6() => ClickCheckboxIfEnabled(Enemy6);
        public void ClickEnemy7() => ClickCheckboxIfEnabled(Enemy7);
        public void ClickEnemy8() => ClickCheckboxIfEnabled(Enemy8);
        public void ClickEnemy9() => ClickCheckboxIfEnabled(Enemy9);

        // Dungeons
        public void ClickDungeon0() => ClickCheckboxIfEnabled(Dungeon0);
        public void ClickDungeon1() => ClickCheckboxIfEnabled(Dungeon1);
        public void ClickDungeon2() => ClickCheckboxIfEnabled(Dungeon2);

        // FATEs
        public void ClickFate0() => ClickCheckboxIfEnabled(Fate0);
        public void ClickFate1() => ClickCheckboxIfEnabled(Fate1);
        public void ClickFate2() => ClickCheckboxIfEnabled(Fate2);


        // Leves
        public void ClickLeve0() => ClickCheckboxIfEnabled(Leve0);
        public void ClickLeve1() => ClickCheckboxIfEnabled(Leve1);
        public void ClickLeve2() => ClickCheckboxIfEnabled(Leve2);

    }
}
