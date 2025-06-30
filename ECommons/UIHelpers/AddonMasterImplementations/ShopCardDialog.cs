using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ShopCardDialog : AddonMasterBase<AddonShopCardDialog>
    {
        public ShopCardDialog(nint addon) : base(addon)
        {
        }

        public ShopCardDialog(void* addon) : base(addon)
        {
        }

        public SeString Price => GenericHelpers.ReadSeString(&Base->GetTextNodeById(10)->NodeText);
        public int? Quantity { get => Addon->CardQuantityInput->Data.Value; set => Addon->CardQuantityInput->SetValue(value.HasValue ? (int)value : MinQuantity); }
        public int MinQuantity => Addon->CardQuantityInput->Data.Min;
        public int MaxQuantity => Addon->CardQuantityInput->Data.Max;

        public AtkComponentButton* SellButton => Base->GetComponentButtonById(16);
        public AtkComponentButton* CancelButton => Base->GetComponentButtonById(17);

        public override string AddonDescription { get; } = "Triple triad card exchange window";

        public void Sell() => ClickButtonIfEnabled(SellButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
