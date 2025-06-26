using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    /// <summary>
    /// Item dyeing addon
    /// </summary>
    public class ColorantColoring : AddonMasterBase<AtkUnitBase>
    {
        public ColorantColoring(nint addon) : base(addon) { }
        public ColorantColoring(void* addon) : base(addon) { }

        public uint ItemId => Addon->AtkValues[2].UInt;
        public int ItemIconId => Addon->AtkValues[3].Int;
        public string ItemName => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[4].String.Value).GetText();

        public AtkComponentButton* ApplyButton => Base->GetComponentButtonById(68);
        public AtkComponentButton* SelectAnotherButton => Base->GetComponentButtonById(69);

        public override string AddonDescription { get; } = "Item dyeing window";

        public void Apply() => ClickButtonIfEnabled(ApplyButton);
        public void SelectAnother() => ClickButtonIfEnabled(SelectAnotherButton);
    }
}