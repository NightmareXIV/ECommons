using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class CompanyCraftSupply : AddonMasterBase<AtkUnitBase>
    {
        public CompanyCraftSupply(nint addon) : base(addon) { }

        public CompanyCraftSupply(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Addon->GetComponentButtonById(41);

        public override string AddonDescription { get; } = "Free Company workshop delivery";

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}