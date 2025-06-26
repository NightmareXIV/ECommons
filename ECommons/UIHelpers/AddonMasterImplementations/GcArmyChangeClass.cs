using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class GcArmyChangeClass : AddonMasterBase<AtkUnitBase>
    {
        public GcArmyChangeClass(nint addon) : base(addon) { }
        public GcArmyChangeClass(void* addon) : base(addon) { }

        public AtkComponentButton* GladiatorButton => Addon->GetComponentButtonById(6);
        public AtkComponentButton* MarauderButton => Addon->GetComponentButtonById(7);
        public AtkComponentButton* PugilistButton => Addon->GetComponentButtonById(11);
        public AtkComponentButton* LancerButton => Addon->GetComponentButtonById(12);
        public AtkComponentButton* RogueButton => Addon->GetComponentButtonById(13);
        public AtkComponentButton* ArcherButton => Addon->GetComponentButtonById(14);
        public AtkComponentButton* ConjurerButton => Addon->GetComponentButtonById(18);
        public AtkComponentButton* ThaumaturgeButton => Addon->GetComponentButtonById(19);
        public AtkComponentButton* ArcanistButton => Addon->GetComponentButtonById(20);

        public override string AddonDescription { get; } = "Squadron change class window";

        public void Gladiator() => ClickButtonIfEnabled(GladiatorButton);
        public void Marauder() => ClickButtonIfEnabled(MarauderButton);
        public void Pugilist() => ClickButtonIfEnabled(PugilistButton);
        public void Lancer() => ClickButtonIfEnabled(LancerButton);
        public void Rogue() => ClickButtonIfEnabled(RogueButton);
        public void Archer() => ClickButtonIfEnabled(ArcherButton);
        public void Conjurer() => ClickButtonIfEnabled(ConjurerButton);
        public void Thaumaturge() => ClickButtonIfEnabled(ThaumaturgeButton);
        public void Arcanist() => ClickButtonIfEnabled(ArcanistButton);
    }
}
