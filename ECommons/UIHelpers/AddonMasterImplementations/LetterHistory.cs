using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class LetterHistory : AddonMasterBase<AtkUnitBase>
    {
        public LetterHistory(nint addon) : base(addon) { }
        public LetterHistory(void* addon) : base(addon) { }

        public AtkComponentButton* CloseButton => Base->GetButtonNodeById(5);

        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
