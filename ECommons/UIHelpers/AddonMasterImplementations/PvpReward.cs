using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// PvP Series rewards addon
    /// </summary>
    public unsafe class PvpReward : AddonMasterBase<AtkUnitBase>
    {
        public PvpReward(nint addon) : base(addon) { }
        public PvpReward(void* addon) : base(addon) { }

        public uint CurrentLevelXp => Addon->AtkValues[10].UInt;
        public uint TotalLevelXp => Addon->AtkValues[11].UInt;

        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(124);

        public override string AddonDescription { get; } = "PvP Series rewards window";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
