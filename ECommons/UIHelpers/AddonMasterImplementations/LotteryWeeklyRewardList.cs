using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LotteryWeeklyRewardList : AddonMasterBase<AtkUnitBase>
    {
        public LotteryWeeklyRewardList(nint addon) : base(addon) { }
        public LotteryWeeklyRewardList(void* addon) : base(addon) { }

        public string Week => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[1].String.Value).GetText();
        public int WinningNumber => Addon->AtkValues[5].Int;

        public AtkComponentButton* CloseButton => Base->GetButtonNodeById(49);

        public void Close() => ClickButtonIfEnabled(CloseButton);

        public Reward[] Rewards
        {
            get
            {
                var ret = new Reward[5];
                for(var i = 0; i < ret.Length; i++)
                    ret[i] = new(this, i);
                return ret;
            }
        }

        public override string AddonDescription { get; } = "Jumbo Cactpot result window";

        public readonly struct Reward(LotteryWeeklyRewardList am, int index)
        {
            public bool Unk01 { get; init; } = am.Addon->AtkValues[10 + 7 * index].Bool;
            public string Place { get; init; } = MemoryHelper.ReadSeStringNullTerminated((nint)am.Addon->AtkValues[11 + 7 * index].String.Value).GetText();
            public int MGPReward { get; init; } = am.Addon->AtkValues[12 + 7 * index].Int;
            public int ItemRewardId { get; init; } = am.Addon->AtkValues[13 + 7 * index].Int;
            public int? ItemRewardIconId { get; init; } = am.Addon->AtkValues[14 + 7 * index].Type == 0 ? null : am.Addon->AtkValues[14 + 7 * index].Int;
            public string? ItemRewardName { get; init; } = am.Addon->AtkValues[15 + 7 * index].Type == 0 ? null : MemoryHelper.ReadSeStringNullTerminated((nint)am.Addon->AtkValues[15 + 7 * index].String.Value).GetText();
            public string Requirement { get; init; } = MemoryHelper.ReadSeStringNullTerminated((nint)am.Addon->AtkValues[16 + 7 * index].String.Value).GetText();
        }

        // TODO: Particpant Rewards struct
    }
}