using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster.Synthesis;

namespace ECommons.UIHelpers.AtkReaderImplementations;
public unsafe class ReaderSynthesis(AtkUnitBase* Addon) : AtkReader(Addon)
{
    public uint Unk0 => ReadUInt(0) ?? 0;
    public bool IsTrialSynthesis => ReadBool(1) ?? false;
    public SeString ItemName => ReadSeString(2);
    public uint ItemIconId => ReadUInt(3) ?? 0;
    public uint ItemCount => ReadUInt(4) ?? 0;
    public uint Progress => ReadUInt(5) ?? 0;
    public uint MaxProgress => ReadUInt(6) ?? 0;
    public uint Durability => ReadUInt(7) ?? 0;
    public uint MaxDurability => ReadUInt(8) ?? 0;
    public uint Quality => ReadUInt(9) ?? 0;
    public uint HQChance => ReadUInt(10) ?? 0;
    private uint IsShowingCollectibleInfoValue => ReadUInt(11) ?? 0;
    private uint ConditionValue => ReadUInt(12) ?? 0;
    public SeString ConditionName => ReadSeString(13);
    public SeString ConditionNameAndTooltip => ReadSeString(14);
    public uint StepCount => ReadUInt(15) ?? 0;
    public uint ResultItemId => ReadUInt(16) ?? 0;
    public uint MaxQuality => ReadUInt(17) ?? 0;
    public uint RequiredQuality => ReadUInt(18) ?? 0;
    private uint IsCollectibleValue => ReadUInt(19) ?? 0;
    public uint Collectability => ReadUInt(20) ?? 0;
    public uint MaxCollectability => ReadUInt(21) ?? 0;
    public uint CollectabilityCheckpoint1 => ReadUInt(22) ?? 0;
    public uint CollectabilityCheckpoint2 => ReadUInt(23) ?? 0;
    public uint CollectabilityCheckpoint3 => ReadUInt(24) ?? 0;
    public bool IsExpertRecipe => ReadBool(25) ?? false;

    public bool IsShowingCollectibleInfo => IsShowingCollectibleInfoValue != 0;
    public Condition Condition => (Condition)ConditionValue;
    public bool IsCollectible => IsCollectibleValue != 0;
    public bool IsMaxProgress => Progress == MaxProgress;
    public bool IsMaxQuality => Quality == MaxQuality;
}