using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class GearSetList : AddonMasterBase<AtkUnitBase>
    {
        public GearSetList(nint addon) : base(addon) { }
        public GearSetList(void* addon) : base(addon) { }

        public AtkComponentButton* DisplayHelpButton => Base->GetButtonNodeById(2);
        public AtkComponentButton* CreateNewGearsetButton => Base->GetButtonNodeById(3);
        public AtkComponentButton* RefreshButton => Base->GetButtonNodeById(4);
        public AtkComponentButton* EquipSetButton => Base->GetButtonNodeById(9);

        public override string AddonDescription { get; } = "Gearset window";

        public void DisplayHelp() => ClickButtonIfEnabled(DisplayHelpButton);
        public void CreateNewGearset() => ClickButtonIfEnabled(CreateNewGearsetButton);
        public void Refresh() => ClickButtonIfEnabled(RefreshButton);
        public void EquipSet() => ClickButtonIfEnabled(EquipSetButton);
    }
}