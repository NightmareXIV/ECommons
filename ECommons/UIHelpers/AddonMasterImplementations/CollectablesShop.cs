using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe class CollectablesShop : AddonMasterBase<AtkUnitBase>
    {
        public CollectablesShop(nint addon) : base(addon) { }

        public CollectablesShop(void* addon) : base(addon) { }

        public AtkComponentButton* TradeButton => Addon->GetButtonNodeById(51);
        public AtkComponentRadioButton* CarpenterButton => Addon->GetComponentNodeById(3)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* BlacksmithButton => Addon->GetComponentNodeById(4)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* ArmourerButton => Addon->GetComponentNodeById(5)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* GoldsmithButton => Addon->GetComponentNodeById(6)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* LeatherworkerButton => Addon->GetComponentNodeById(7)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* WeaverButton => Addon->GetComponentNodeById(8)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* AlchemistButton => Addon->GetComponentNodeById(9)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* CulinarianButton => Addon->GetComponentNodeById(10)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* MinerButton => Addon->GetComponentNodeById(11)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* BotanistButton => Addon->GetComponentNodeById(12)->GetAsAtkComponentRadioButton();
        public AtkComponentRadioButton* FisherButton => Addon->GetComponentNodeById(13)->GetAsAtkComponentRadioButton();

        public override string AddonDescription { get; } = "Collectables";

        public void Trade() => ClickButtonIfEnabled(TradeButton);

        public bool SelectDiscipleTab(Job job) => SelectDiscipleTab((uint)job);
        public bool SelectDiscipleTab(uint classjob) => classjob is >= 8 and <= 18 ? ClickButtonIfEnabled(Addon->GetComponentNodeById(classjob - 5)->GetAsAtkComponentRadioButton()) : throw new ArgumentOutOfRangeException(nameof(classjob));
    }
}
