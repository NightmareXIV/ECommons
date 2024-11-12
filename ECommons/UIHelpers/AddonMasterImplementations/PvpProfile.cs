using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class PvpProfile : AddonMasterBase<AtkUnitBase>
    {
        public PvpProfile(nint addon) : base(addon) { }
        public PvpProfile(void* addon) : base(addon) { }

        public AtkComponentButton* SeriesMalmstonesButton => Addon->GetButtonNodeById(21);

        public override string AddonDescription { get; } = "PvP Profile";

        public void SeriesMalmstones() => ClickButtonIfEnabled(SeriesMalmstonesButton);
    }
}
