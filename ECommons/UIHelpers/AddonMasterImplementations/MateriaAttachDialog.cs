using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MateriaAttachDialog : AddonMasterBase<AtkUnitBase>
    {
        public MateriaAttachDialog(nint addon) : base(addon)
        {
        }

        public MateriaAttachDialog(void* addon) : base(addon) { }

        public int SuccessRate => Base->AtkValues[41].Int;
        public SeString SuccessRateSeString => GenericHelpers.ReadSeString(&Base->GetTextNodeById(26)->NodeText);
        public string SuccessRateText => SuccessRateSeString.GetText();
        public float SuccessRateFloat => SuccessRateText.Where(char.IsDigit).Append('.').Append('-').Any()
                            ? float.Parse(string.Join("", SuccessRateText.Where(char.IsDigit).Append('.').Append('-')))
                            : 0.0f;

        public AtkComponentButton* MeldButton => Base->GetButtonNodeById(35);
        public AtkComponentButton* ReturnButton => Base->GetButtonNodeById(36);

        public override string AddonDescription { get; } = "Materia melding window";

        public void Meld() => ClickButtonIfEnabled(MeldButton);
        public void Return() => ClickButtonIfEnabled(ReturnButton);
    }
}
