using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LotteryWeeklyInput : AddonMasterBase<AtkUnitBase>
    {
        public LotteryWeeklyInput(nint addon) : base(addon) { }
        public LotteryWeeklyInput(void* addon) : base(addon) { }

        public int Week => Addon->AtkValues[0].Int;
        public bool Unk03 => Addon->AtkValues[3].Bool;
        public int Unk04 => Addon->AtkValues[4].Int;
        public int Unk05 => Addon->AtkValues[5].Int;

        public AtkComponentButton* PurchaseButton => Base->GetButtonNodeById(31);
        public AtkComponentButton* RandomButton => Base->GetButtonNodeById(32);

        public override string AddonDescription { get; } = "Jumbo Cactpot ticket purchase window";

        public void Purchase() => ClickButtonIfEnabled(PurchaseButton);
        public void Random() => ClickButtonIfEnabled(RandomButton);
    }
}