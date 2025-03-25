using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Portraits list addon
    /// </summary>
    public unsafe class BannerList : AddonMasterBase<AtkUnitBase>
    {
        public BannerList(nint addon) : base(addon) { }
        public BannerList(void* addon) : base(addon) { }

        public AtkComponentButton* EditButton => Addon->GetButtonNodeById(2);
        public AtkComponentButton* DisplayHelpButton => Addon->GetButtonNodeById(8);
        public AtkComponentButton* UseAsInstantPortraitButton => Addon->GetButtonNodeById(34);

        public int NumPortraits => Addon->AtkValues[17].Int;
        public int SelectedPortrait => Addon->AtkValues[18].Int; // 0 indexed
        public int CharacterOption1IconId => Addon->AtkValues[6].Int;
        public int CharacterOption2IconId => Addon->AtkValues[8].Int;
        public int BackgroundIconId => Addon->AtkValues[10].Int;
        public int FrameIconId => Addon->AtkValues[12].Int;
        public int AccentIconId => Addon->AtkValues[14].Int;

        public Portraits[] Portrait
        {
            get
            {
                var ret = new Portraits[NumPortraits];
                for(var i = 0; i < ret.Length; i++)
                    ret[i] = new(Addon, Addon->AtkValues[23 + 7 * i].Int);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Portraits list";

        public readonly struct Portraits
        {
            private readonly AtkUnitBase* Addon;
            public uint Unk01 { get; init; } // always 0?
            public int ClassJobIconId { get; init; }

            /// <summary>
            /// 1-based index
            /// </summary>
            public int ListIndex { get; init; }
            public int GlamourPlateId { get; init; } // 0 = no link

            /// <summary>
            /// 1 = broken <br></br>
            /// 0 = not broken
            /// </summary>
            public int PortraitBroken { get; init; }

            /// <summary>
            /// 7 = unable to retrieve glamour plate data <br></br>
            /// 5 = broken portrait <br></br>
            /// 1 = unbroken portrait and UseAsInstantPortrait is off <br></br>
            /// 0 = unbroken portrait and UseAsInstantPortrait is on
            /// </summary>
            public int Unk06 { get; init; }

            /// <summary>
            /// 0 = on <br></br>
            /// 1 = off
            /// </summary>
            public int UseAsInstantPortrait { get; init; }
            public SeString GearSetName { get; init; }
            public SeString GearSetILvl { get; init; }

            public bool IsPortraitBroken => PortraitBroken == 1;
            public bool IsUseAsInstantPortraitSet => UseAsInstantPortrait == 0;

            public Portraits(AtkUnitBase* addon, int index)
            {
                Addon = addon;
                ListIndex = index;

                var offset = 7 * (ListIndex - 1);
                Unk01 = Addon->AtkValues[21 + offset].UInt;
                ClassJobIconId = Addon->AtkValues[22 + offset].Int;
                GlamourPlateId = Addon->AtkValues[24 + offset].Int;
                PortraitBroken = Addon->AtkValues[25 + offset].Int;
                Unk06 = Addon->AtkValues[26 + offset].Int;
                UseAsInstantPortrait = Addon->AtkValues[27 + offset].Int;

                var offset2 = 2 * (ListIndex - 1);
                GearSetName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[791 + offset2].String.Value);
                GearSetILvl = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[792 + offset2].String.Value);
            }
        }

        public void Edit() => ClickButtonIfEnabled(EditButton);
        public void DisplayHelp() => ClickButtonIfEnabled(DisplayHelpButton);
        public void UseAsInstantPortrait() => ClickButtonIfEnabled(UseAsInstantPortraitButton);
    }
}
