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
                if(Addon->AtkValues[0].IsString())
                {
                    return MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[0].String.Value).GetText();
                }
                return null;
            }
        }

        public uint? CurrentScore
        {
            get
            {
                if(Addon->AtkValuesCount < 2 || !Addon->AtkValues[2].IsString())
                    return null;

                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[2].String.Value);
                if(rawValue == null)
                    return null;

                if(uint.TryParse(rawValue.TextValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue.TextValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return null;
            }
        }

        public uint? SilverScore
        {
            get
            {
                if(Addon->AtkValuesCount < 2 || !Addon->AtkValues[3].IsString())
                    return null;

                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[3].String.Value);
                if(rawValue == null)
                    return null;

                if(uint.TryParse(rawValue.TextValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue.TextValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return null;
            }
        }

        public uint? GoldScore
        {
            get
            {
                if(Addon->AtkValuesCount < 2 || !Addon->AtkValues[4].IsString())
                    return null;

                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[4].String.Value);
                if(rawValue == null)
                    return null;

                if(uint.TryParse(rawValue.TextValue, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(rawValue.TextValue, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return null;
            }
        }

        public uint? CriticalScore
        {
            get
            {
                if(Addon->AtkValuesCount < 2 || !Addon->AtkValues[5].IsString())
                    return null;

                var rawValue = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[5].String.Value);
                if(rawValue == null)
                    return null;

                var text = rawValue.TextValue;

                if(text.Contains("/"))
                {
                    var parts = text.Split('/');
                    if(parts.Length == 2 && uint.TryParse(parts[0].Trim(), out var numerator))
                        return numerator;
                }

                if(uint.TryParse(text, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return result;

                var cleanedValue = System.Text.RegularExpressions.Regex.Replace(text, @"[^\d]", "");
                if(uint.TryParse(cleanedValue, out result))
                    return result;

                return null;
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
