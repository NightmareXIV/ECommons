using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroupPrivate : AddonMasterBase<AtkUnitBase>
    {
        public LookingForGroupPrivate(nint addon) : base(addon) { }
        public LookingForGroupPrivate(void* addon) : base(addon) { }

        public AtkComponentButton* JoinButton => Base->GetButtonNodeById(6);
        public AtkComponentButton* CancelButton => Base->GetButtonNodeById(7);

        public override string AddonDescription { get; } = "Private party finder password prompt";

        public void Join(int password) => Callback.Fire(Addon, true, 0, password);
        public void Join() => ClickButtonIfEnabled(JoinButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
