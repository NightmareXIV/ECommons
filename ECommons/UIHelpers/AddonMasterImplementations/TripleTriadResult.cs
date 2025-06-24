using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class TripleTriadResult : AddonMasterBase<AtkUnitBase>
    {
        public TripleTriadResult(nint addon) : base(addon) { }
        public TripleTriadResult(void* addon) : base(addon) { }

        /// <summary>
        /// 0 = won, 1 = lost
        /// </summary>
        public int WonValue => Addon->AtkValues[2].Int;
        public uint MGPReward => Addon->AtkValues[7].UInt;
        public bool WonGame => WonValue == 0;

        public AtkComponentButton* RematchButton => Addon->GetComponentButtonById(21);
        public AtkComponentButton* QuitButton => Addon->GetComponentButtonById(22);

        public override string AddonDescription { get; } = "Triple triad result window";

        public void Rematch() => ClickButtonIfEnabled(RematchButton);
        public void Quit() => ClickButtonIfEnabled(QuitButton);
    }
}
