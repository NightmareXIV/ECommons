using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class TreasureHighLow : AddonMasterBase<AtkUnitBase>
    {
        public TreasureHighLow(nint addon) : base(addon) { }
        public TreasureHighLow(void* addon) : base(addon) { }

        public override string AddonDescription { get; } = "Treasure map dungeon Gambler's Lure";

        /// <remarks>
        /// Only visible when it's your chest
        /// </remarks>
        public AtkComponentButton* TryLuckButton => Addon->GetButtonNodeById(46);
        /// <remarks>
        /// Only visible when it's your chest
        /// </remarks>
        public AtkComponentButton* OpenChestButton => Addon->GetButtonNodeById(47);
        /// <remarks>
        /// Only visible when it's not your chest
        /// </remarks>
        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(48);

        /// <remarks>
        /// Only callable when it's your chest
        /// </remarks>
        public void TryLuck() => ClickButtonIfEnabled(TryLuckButton);
        /// <remarks>
        /// Only callable when it's your chest
        /// </remarks>
        public void OpenChest() => ClickButtonIfEnabled(OpenChestButton);
        /// <remarks>
        /// Only callable when it's not your chest
        /// </remarks>
        public void Close() => ClickButtonIfEnabled(CloseButton);
    }
}