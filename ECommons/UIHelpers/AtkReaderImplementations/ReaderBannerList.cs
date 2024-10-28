using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderBannerList(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public int CharacterOption1IconId => ReadInt(6) ?? 0;
    public int CharacterOption2IconId => ReadInt(8) ?? 0;
    public int BackgroundIconId => ReadInt(10) ?? 0;
    public int FrameIconId => ReadInt(12) ?? 0;
    public int AccentIconId => ReadInt(14) ?? 0;
    public int NumPortraits => ReadInt(17) ?? 0;
    public int SelectedPortrait => ReadInt(18) ?? 0; // 0 indexed

    public List<Portrait> Portraits => Loop<Portrait>(21, 7, 100);

    public unsafe class Portrait(nint Addon, int start) : AtkReader(Addon, start)
    {
        public uint Unk01 => ReadUInt(21) ?? 0; // always 0?
        public int ClassJobIconId => ReadInt(22) ?? 0;

        /// <summary>
        /// 1-based index
        /// </summary>
        public int ListIndex => ReadInt(23) ?? 0;
        public int GlamourPlateId => ReadInt(24) ?? 0; // 0 = no link

        /// <summary>
        /// 1 = broken <br></br>
        /// 0 = not broken
        /// </summary>
        public int PortraitBroken => ReadInt(25) ?? 0;

        /// <summary>
        /// 7 = unable to retrieve glamour plate data <br></br>
        /// 5 = broken portrait <br></br>
        /// 1 = unbroken portrait and UseAsInstantPortrait is off <br></br>
        /// 0 = unbroken portrait and UseAsInstantPortrait is on
        /// </summary>
        public int Unk06 => ReadInt(26) ?? 0;

        /// <summary>
        /// 0 = on <br></br>
        /// 1 = off
        /// </summary>
        public int UseAsInstantPortrait => ReadInt(27) ?? 0;
        public SeString GearSetName => ReadSeString(791);
        public SeString GearSetILvl => ReadSeString(792);

        public bool IsPortraitBroken => PortraitBroken == 1;
        public bool IsUseAsInstantPortraitSet => UseAsInstantPortrait == 0;
    }
}
