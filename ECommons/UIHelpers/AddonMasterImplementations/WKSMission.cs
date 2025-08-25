using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using Callback = ECommons.Automation.Callback;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    /// <summary>
    /// Space Exploration Mission Screen <br></br>
    /// Details all the missions that you can pick up/have done
    /// </summary>
    public unsafe partial class WKSMission : AddonMasterBase<AtkUnitBase>
    {
        public WKSMission(nint addon) : base(addon) { }
        public WKSMission(void* addon) : base(addon) { }

        public AtkComponentButton* HelpButton => Addon->GetComponentButtonById(7);
        public AtkComponentButton* MissionSelectionButton => Addon->GetComponentButtonById(8);
        public AtkComponentButton* MissionLogButton => Addon->GetComponentButtonById(9);
        public AtkComponentButton* BasicMissionsButton => Addon->GetComponentButtonById(13);
        public AtkComponentButton* ProvisionalMissionsButton => Addon->GetComponentButtonById(14);
        public AtkComponentButton* CriticalMissionsButton => Addon->GetComponentButtonById(15);

        /// <summary>
        /// Keeps the current number of missions that are displayed. <br></br>
        /// This includes the tabs seperating the missions by type [A, B, C, D]
        /// </summary>
        public uint NumEntries => Addon->AtkValues[31].UInt;

        public uint SelectedMissionId => Addon->AtkValues[1061].UInt;
        public string SelectedMissionName
        {
            get
            {
                var missionName = Addon->AtkValues[1062];
                if(missionName.Type.EqualsAny(ValueType.String, ValueType.ManagedString, ValueType.String8))
                {
                    return MemoryHelper.ReadSeStringNullTerminated((nint)missionName.String.Value).GetText();
                }
                return "n/a";
            }
        }

        public StellarMissions[] StellerMissions
        {
            get
            {
                var ret = new List<StellarMissions>();
                for(var i = 0; i < NumEntries; i++)
                {
                    var missionName = Addon->AtkValues[802 + i * 2];
                    var missionId = Addon->AtkValues[40 + i * 6].UInt;

                    // category header?
                    if(missionId == 0)
                        continue;

                    if(missionName.Type.EqualsAny(ValueType.String, ValueType.ManagedString, ValueType.String8))
                    {
                        var mission = new StellarMissions(this, i)
                        {
                            Name = MemoryHelper.ReadSeStringNullTerminated((nint)missionName.String.Value).GetText(),
                            MissionId = missionId
                        };
                        ret.Add(mission);
                    }
                    else
                    {
                        break;
                    }
                }
                return [.. ret];
            }
        }

        public class StellarMissions(WKSMission master, int index)
        {
            public string Name { get; set; } = string.Empty;
            public uint MissionId;

            public void Select()
            {
                Callback.Fire(master.Base, true, 12, (int)MissionId, index);
            }
            public void Initiate()
            {
                Callback.Fire(master.Base, true, 13, (int)MissionId, index);
            }
        }

        public override string AddonDescription => "Steller Missions Ui";

        public void Help() => ClickButtonIfEnabled(HelpButton);
        public void MissionSelection() => ClickButtonIfEnabled(MissionSelectionButton);
        public void MissionLog() => ClickButtonIfEnabled(MissionLogButton);
        public void BasicMissions() => ClickButtonIfEnabled(BasicMissionsButton);
        public void ProvisionalMissions() => ClickButtonIfEnabled(ProvisionalMissionsButton);
        public void CriticalMissions() => ClickButtonIfEnabled(CriticalMissionsButton);
    }
}
