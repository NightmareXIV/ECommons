using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Globalization;

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

        public string Name
        {
            get
            {
                return MemoryHelper
                    .ReadSeStringNullTerminated((nint)Addon->AtkValues[0].String.Value)
                    .GetText();
            }
        }

        public uint CurrentScore
        {
            get
            {
                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[2].String.Value).GetText();

                // Number coversion test #1.
                if(uint.TryParse(rawValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                // Fallback: if the first test fails
                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return 0; // fallback if parsing fails
            }
        }

        public uint SilverScore
        {
            get
            {
                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[3].String.Value).GetText();

                // Number coversion test #1.
                if(uint.TryParse(rawValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                // Fallback: if the first test fails
                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return 0; // fallback if parsing fails
            }
        }

        public uint GoldScore
        {
            get
            {
                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[4].String.Value).GetText();

                // Number coversion test #1.
                if(uint.TryParse(rawValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                // Fallback: if the first test fails
                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return 0; // fallback if parsing fails
            }
        }

        public uint CriticalScore
        {
            get
            {
                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[5].String.Value).GetText();

                // Extract the left side of the slash
                var leftSide = rawValue.Split('/')[0].Trim();

                // Number conversion test #1.
                if(uint.TryParse(leftSide, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                // Fallback: if the first test fails
                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(leftSide, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return 0; // fallback if parsing fails
            }
        }

        public AtkComponentButton* CosmoPouchButton => Addon->GetComponentButtonById(26);
        public AtkComponentButton* CosmoCraftingLogButton => Addon->GetComponentButtonById(27);
        public AtkComponentButton* StellerReductionButton => Addon->GetComponentButtonById(28);
        public AtkComponentButton* ReportResultsButton => Addon->GetComponentButtonById(29);
        public AtkComponentButton* AbandonMissionButton => Addon->GetComponentButtonById(30);

        public void CosmoPouch() => ClickButtonIfEnabled(CosmoPouchButton);
        public void CosmoCraftingLog() => ClickButtonIfEnabled(CosmoCraftingLogButton);
        public void StellerReduction() => ClickButtonIfEnabled(StellerReductionButton);
        public void Report() => ClickButtonIfEnabled(ReportResultsButton);
        public void Abandon() => ClickButtonIfEnabled(AbandonMissionButton);

        public override string AddonDescription => "Cosmic Exploration Mission Information";
    }
}
