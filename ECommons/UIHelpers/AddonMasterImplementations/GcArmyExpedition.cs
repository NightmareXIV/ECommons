using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class GcArmyExpedition : AddonMasterBase<AddonGcArmyExpedition>
    {
        public GcArmyExpedition(nint addon) : base(addon) { }
        public GcArmyExpedition(void* addon) : base(addon) { }

        public void Deploy() => ClickButtonIfEnabled(Addon->DeployButton);
    }
}
