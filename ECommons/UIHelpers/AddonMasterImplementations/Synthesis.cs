using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class Synthesis : AddonMasterBase<AtkUnitBase>
    {
        public Synthesis(nint addon) : base(addon) { }
        public Synthesis(void* addon) : base(addon) { }

        public ReaderSynthesis Reader => new(Base);

        public override string AddonDescription { get; } = "Crafting synthesis window";

        public enum Condition
        {
            Normal,
            Good,
            Excellent,
            Poor,
            Centered,
            Sturdy,
            Pliant,
            Malleable,
            Primed,
            GoodOmen,

            Unknown
        }
    }
}
