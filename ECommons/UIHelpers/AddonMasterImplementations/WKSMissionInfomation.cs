using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Mission information screen
    /// Can be viewed post you grabbing a moon mission
    /// </summary>
    public unsafe partial class WKSMissionInfomation : AddonMasterBase<AtkUnitBase>
    {
        public WKSMissionInfomation(nint addon) : base(addon) { }
        public WKSMissionInfomation(void* addon) : base(addon) { }

        public uint CurrentScore
        {
            get
            {
                string rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[2].String.Value).GetText();
                rawValue = rawValue.Replace(",", ""); // remove thousand separators
                if(uint.TryParse(rawValue, out uint result))
                    return result;
                return 0; // fallback if parsing fails
            }
        }

        public uint SilverScore
        {
            get
            {
                string rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[3].String.Value).GetText();
                rawValue = rawValue.Replace(",", ""); // remove thousand separators
                if(uint.TryParse(rawValue, out uint result))
                    return result;
                return 0; // fallback if parsing fails
            }
        }

        public uint GoldScore
        {
            get
            {
                string rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[4].String.Value).GetText();
                rawValue = rawValue.Replace(",", ""); // remove thousand separators
                if(uint.TryParse(rawValue, out uint result))
                    return result;
                return 0; // fallback if parsing fails
            }
        }

        public AtkComponentButton* CosmoPouchButton => Addon->GetButtonNodeById(26);
        public AtkComponentButton* CosmoCraftingLogButton => Addon->GetButtonNodeById(27);
        public AtkComponentButton* StellerReductionButton => Addon->GetButtonNodeById(28);
        public AtkComponentButton* ReportResultsButton => Addon->GetButtonNodeById(29);
        public AtkComponentButton* AbandonMissionButton => Addon->GetButtonNodeById(30);

        public void CosmoPouch() => ClickButtonIfEnabled(CosmoPouchButton);
        public void CosmoCraftingLog() => ClickButtonIfEnabled(CosmoCraftingLogButton);
        public void StellerReduction() => ClickButtonIfEnabled(StellerReductionButton);
        public void Report() => ClickButtonIfEnabled(ReportResultsButton);
        public void Abandon() => ClickButtonIfEnabled(AbandonMissionButton);

        public override string AddonDescription => "Cosmic Exploration Mission Information";
    }
}
