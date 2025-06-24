using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;


namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class PurifyAutoDialog : AddonMasterBase<AtkUnitBase>
    {
        public PurifyAutoDialog(nint addon) : base(addon) { }

        public PurifyAutoDialog(void* addon) : base(addon) { }

        public AtkComponentButton* CancelExitButton => Addon->GetComponentButtonById(16);
        public SeString CancelExitButtonSeString => GenericHelpers.ReadSeString(&CancelExitButton->UldManager.SearchNodeById(2)->GetAsAtkTextNode()->NodeText);
        public string CancelExitButtonText => CancelExitButtonSeString.GetText();
        public bool PurificationActive => Svc.Data.GetExcelSheet<Addon>()!.GetRow(3868)!.Text.ToString().Equals(CancelExitButtonText);
        public bool PurificationInactive => Svc.Data.GetExcelSheet<Addon>()!.GetRow(3869)!.Text.ToString().Equals(CancelExitButtonText);

        public override string AddonDescription { get; } = "Aetherial Reduction Dialog";

        public void CancelExit() => ClickButtonIfEnabled(CancelExitButton);
    }
}
