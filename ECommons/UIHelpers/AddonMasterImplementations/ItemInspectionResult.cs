using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ItemInspectionResult : AddonMasterBase<AddonItemInspectionResult>
    {
        public ItemInspectionResult(nint addon) : base(addon)
        {
        }

        public ItemInspectionResult(void* addon) : base(addon) { }

        public AtkComponentButton* NextButton => Base->GetButtonNodeById(73);
        public AtkComponentButton* CloseButton => Base->GetButtonNodeById(74);

        public void Next() => ClickButtonIfEnabled(NextButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}
