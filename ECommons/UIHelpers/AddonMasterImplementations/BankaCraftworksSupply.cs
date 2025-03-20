using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Crystarium/Studium/Wachumeqimeqi Deliveries addon
    /// </summary>
    public unsafe class BankaCraftworksSupply : AddonMasterBase<AtkUnitBase>
    {
        public BankaCraftworksSupply(nint addon) : base(addon) { }
        public BankaCraftworksSupply(void* addon) : base(addon) { }

        public uint CollectableItemId => Addon->AtkValues[8].UInt;

        public AtkComponentButton* DeliverButton => Addon->GetButtonNodeById(71);
        public AtkComponentButton* CancelButton => Addon->GetButtonNodeById(72);

        public void Deliver() => ClickButtonIfEnabled(DeliverButton);
        public void Cancel() => ClickButtonIfEnabled(CancelButton);

        public int RequestedItemNumberAvailable => InventoryManager.Instance()->GetInventoryItemCount(CollectableItemId);
        public List<int> SlotsFilled => [.. Enumerable.Range(0, 6).Where(i => Addon->GetComponentNodeById((uint)(i + 36))->Component->UldManager.NodeList[1]->IsVisible()).Select(i => i)];
        public int? FirstUnfilledSlot => SlotsFilled.Count == 6 ? null : Enumerable.Range(0, 6).FirstOrDefault(i => !SlotsFilled.Contains(i));

        public override string AddonDescription { get; } = "Crystarium/Studium/Wachumeqimeqi Deliveries window";

        public bool? TryHandOver(int slot)
        {
            if(SlotsFilled.Contains(slot)) return true;

            var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ContextIconMenu", 1);

            if(contextMenu is null || !contextMenu->IsVisible)
            {
                Callback.Fire(Base, true, 2, slot);
                return false;
            }
            else
            {
                Callback.Fire(contextMenu, true, 0, 0, 1021003u, 0u, 0);
                PluginLog.Debug($"Filled slot {slot}");
                return true;
            }
        }
    }
}

