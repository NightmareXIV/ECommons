using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class ContentsFinderConfirm : AddonMasterBase<AddonContentsFinderConfirm>
    {
        public override string AddonDescription { get; } = "Duty finder confirmation window";

        public ContentsFinderConfirm(nint addon) : base(addon)
        {
        }

        public ContentsFinderConfirm(void* addon) : base(addon) { }

        public void Commence() => ClickButtonIfEnabled(Addon->CommenceButton);
        public void Withdraw() => ClickButtonIfEnabled(Addon->WithdrawButton);
        public void Wait() => ClickButtonIfEnabled(Addon->WaitButton);
    }
}
