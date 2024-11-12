using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class WorldTravelFinderStatus : AddonMasterBase<AtkUnitBase>
    {
        public WorldTravelFinderStatus(nint addon) : base(addon) { }
        public WorldTravelFinderStatus(void* addon) : base(addon) { }

        public string StartingWorldString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[1].String).ExtractText();
        public string DestinationWorldString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[2].String).ExtractText();
        public string PositionInQueueString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[3].String).ExtractText();
        public string TimeElapsedString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[4].String).ExtractText();
        public string TimeRemainingString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[5].String).ExtractText();
        /* TODO: fix or delete
        public World? StartingWorld => GenericHelpers.FindRow<World>(x => !string.IsNullOrEmpty(x!.Name) && x.Name == StartingWorldString);
        public World? DestinationWorld => GenericHelpers.FindRow<World>(x => !string.IsNullOrEmpty(x!.Name) && x.Name == DestinationWorldString);*/

        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(13);

        public override string AddonDescription => "In-game world travel status window";

        public void Cancel() => ClickButtonIfEnabled(CancelButton);
    }
}
