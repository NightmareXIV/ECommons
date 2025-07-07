using FFXIVClientStructs.FFXIV.Component.GUI;
namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe partial class MJIHud : AddonMasterBase<AtkUnitBase>
    {
        public MJIHud(nint addon) : base(addon) { }
        public MJIHud(void* addon) : base(addon) { }

        public override string AddonDescription { get; } = "Island Sanctuary Main Hud";

        public uint SanctuaryRank => Addon->AtkValues[11].UInt;
        public uint CurrentIslandXP => Addon->AtkValues[12].UInt;
        public uint NextIslandLevelXP => Addon->AtkValues[13].UInt;
        public uint IslandersCowrie => Addon->AtkValues[14].UInt;
        public uint SeafarersCowrie => Addon->AtkValues[17].UInt;


    }
}
