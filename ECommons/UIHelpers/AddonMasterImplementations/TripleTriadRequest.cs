using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using static ECommons.GenericHelpers;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class TripleTriadRequest : AddonMasterBase<AtkUnitBase>
    {
        public TripleTriadRequest(nint addon) : base(addon) { }
        public TripleTriadRequest(void* addon) : base(addon) { }

        public string Opponent => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[7].String.Value).GetText();
        public int CurrentMGP => Addon->AtkValues[9].Int;

        public int RegionalRule1 => Addon->AtkValues[102].Int;
        public int RegionalRule2 => Addon->AtkValues[103].Int;
        public int MatchRule1 => Addon->AtkValues[104].Int;
        public int MatchRule2 => Addon->AtkValues[105].Int;
        //TODO: fix
        //public List<TripleTriadRule> RegionalRules => [GetRow<TripleTriadRule>((uint)RegionalRule1), GetRow<TripleTriadRule>((uint)RegionalRule2)];
        //public List<TripleTriadRule> MatchRules => [GetRow<TripleTriadRule>((uint)MatchRule1), GetRow<TripleTriadRule>((uint)MatchRule2)];

        public int MatchFee => Addon->AtkValues[111].Int;
        public uint MGPReward => Addon->AtkValues[112].UInt;

        public AtkComponentButton* ChallengeButton => Addon->GetComponentButtonById(41);
        public AtkComponentButton* QuitButton => Addon->GetComponentButtonById(42);

        public override string AddonDescription { get; } = "Triple triad challenge window";

        public void Challenge() => ClickButtonIfEnabled(ChallengeButton);
        public void Quit() => ClickButtonIfEnabled(QuitButton);
    }
}
