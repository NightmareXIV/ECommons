using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RetainerSell : AddonMasterBase<AddonRetainerSell>
    {
        public RetainerSell(nint addon) : base(addon) { }
        public RetainerSell(void* addon) : base(addon) { }

        public AtkComponentButton* ComparePricesButton => Addon->GetComponentButtonById(4);
        public AtkComponentButton* ConfirmButton => Addon->GetComponentButtonById(21);
        public AtkComponentButton* CancelButton => Addon->GetComponentButtonById(22);

        public int AskingPrice
        {
            get => Addon->AtkValues[5].Int;
            set => Callback.Fire(Base, true, 2, value);
        }

        public int Quantity
        {
            get => Addon->AtkValues[8].Int;
            set => Callback.Fire(Base, true, 3, value);
        }

        public string ItemName => Addon->GetTextNodeById(7)->NodeText.GetText();

        public override string AddonDescription { get; } = "Retainer item sell window";

        public void ComparePrices() => ClickButtonIfEnabled(ComparePricesButton);
        public void Confirm() => ClickButtonIfEnabled(ConfirmButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
