using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class JournalAccept : AddonMasterBase<AtkUnitBase>
    {
        public JournalAccept(nint addon) : base(addon) { }
        public JournalAccept(void* addon) : base(addon) { }
        public override string AddonDescription { get; } = "Quest accept window";
        public AtkComponentButton* AcceptButton => Addon->GetComponentButtonById(44);
        public AtkComponentButton* DeclineButton => Addon->GetComponentButtonById(45);

        public void Accept() => ClickButtonIfEnabled(AcceptButton);
        public void Decline() => ClickButtonIfEnabled(DeclineButton);
    }
}
