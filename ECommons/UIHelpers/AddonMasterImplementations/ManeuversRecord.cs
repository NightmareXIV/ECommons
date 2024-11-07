using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Rival Wings post-game addon
    /// </summary>
    public unsafe class ManeuversRecord : AddonMasterBase<AtkUnitBase>
    {
        public ManeuversRecord(nint addon) : base(addon) { }
        public ManeuversRecord(void* addon) : base(addon) { }

        public AtkComponentButton* LeaveButton => Addon->GetButtonNodeById(62);

        public override string AddonDescription { get; } = "Rival Wings post-game window";

        public void Leave() => ClickButtonIfEnabled(LeaveButton);
    }
}
