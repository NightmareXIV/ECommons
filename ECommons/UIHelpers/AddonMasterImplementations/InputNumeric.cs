using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class InputNumeric : AddonMasterBase<AtkUnitBase>
    {
        public InputNumeric(nint addon) : base(addon) { }
        public InputNumeric(void* addon) : base(addon) { }

        public uint Min => Addon->AtkValues[2].UInt;
        public uint Max => Addon->AtkValues[3].UInt;

        public AtkComponentButton* OkButton => Addon->GetButtonNodeById(4);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(5);

        public override string AddonDescription { get; } = "Number input dialogue";

        public void Ok(int value) => Callback.Fire(Addon, true, value);
        public void Ok() => ClickButtonIfEnabled(OkButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
