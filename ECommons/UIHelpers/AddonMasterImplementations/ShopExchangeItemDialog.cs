using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ShopExchangeItemDialog : AddonMasterBase<AtkUnitBase>
    {
        public ShopExchangeItemDialog(nint addon) : base(addon) { }
        public ShopExchangeItemDialog(void* addon) : base(addon) { }

        public AtkComponentButton* ExchangeButton => Addon->GetButtonNodeById(18);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(19);

        public override string AddonDescription { get; } = "Non-Gilshop Item Exchange Confirmation Dialog";

        public void Exchange() => ClickButtonIfEnabled(ExchangeButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
