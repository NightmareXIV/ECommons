using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Custom input numeric-esque addon for venture exchanges
    /// </summary>
    public unsafe class ShopExchangeCurrencyDialog : AddonMasterBase<AtkUnitBase>
    {
        public ShopExchangeCurrencyDialog(nint addon) : base(addon) { }
        public ShopExchangeCurrencyDialog(void* addon) : base(addon) { }

        public AtkComponentButton* ExchangeButton => Addon->GetComponentButtonById(17);
        public AtkComponentButton* CancelButton => Addon->GetComponentButtonById(18);

        public override string AddonDescription { get; } = "Venture purchase window";

        public void Exchange() => ClickButtonIfEnabled(ExchangeButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
