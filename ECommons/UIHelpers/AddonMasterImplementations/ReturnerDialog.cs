using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ReturnerDialog : AddonMasterBase<AtkUnitBase>
    {
        public ReturnerDialog(nint addon) : base(addon) { }
        public ReturnerDialog(void* addon) : base(addon) { }

        public AtkComponentButton* AcceptButton => Addon->GetButtonNodeById(4);
        public AtkComponentButton* DeclineButton => Addon->GetButtonNodeById(5);
        public AtkComponentButton* DecideLaterButton => Addon->GetButtonNodeById(6);

        public override string AddonDescription { get; } = "Returner status confirmation window";

        public void Accept() => ClickButtonIfEnabled(AcceptButton);
        public void Decline() => ClickButtonIfEnabled(DeclineButton);
        public void DecideLater() => ClickButtonIfEnabled(DecideLaterButton);
    }
}
