using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SalvageAutoDialog : AddonMasterBase<AtkUnitBase>
    {
        public SalvageAutoDialog(nint addon) : base(addon) { }

        public SalvageAutoDialog(void* addon) : base(addon) { }

        public AtkComponentButton* EndDesynthesisButton => Addon->GetButtonNodeById(28);
        public SeString EndDesynthesisButtonSeString => MemoryHelper.ReadSeString(&EndDesynthesisButton->UldManager.SearchNodeById(2)->GetAsAtkTextNode()->NodeText);
        public string EndDesynthesisButtonText => EndDesynthesisButtonSeString.ExtractText();
        public bool DesynthesisActive => Svc.Data.GetExcelSheet<Addon>()!.GetRow(5867)!.Text.RawString.Equals(EndDesynthesisButtonText);
        public bool DesynthesisInactive => Svc.Data.GetExcelSheet<Addon>()!.GetRow(5868)!.Text.RawString.Equals(EndDesynthesisButtonText);

        public void EndDesynthesis() => ClickButtonIfEnabled(EndDesynthesisButton);
    }
}
