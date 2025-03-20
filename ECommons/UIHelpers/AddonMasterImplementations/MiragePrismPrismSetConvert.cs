using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class MiragePrismPrismSetConvert : AddonMasterBase<AtkUnitBase>
    {
        public MiragePrismPrismSetConvert(nint addon) : base(addon) { }
        public MiragePrismPrismSetConvert(void* addon) : base(addon) { }
        public override string AddonDescription { get; } = "Outfit glamour creation";

        public AtkComponentButton* StoreAsGlamourButton => Addon->GetButtonNodeById(27);
        public AtkComponentButton* CloseButton => Addon->GetButtonNodeById(26);

        public void StoreAsGlamour() => ClickButtonIfEnabled(StoreAsGlamourButton);
        public void Close() => ClickButtonIfEnabled(CloseButton);

        public class Item(ReaderMiragePrismPrismSetConvert.Item handle) : ReaderMiragePrismPrismSetConvert.Item(handle.AtkReaderParams.UnitBase, handle.AtkReaderParams.BeginOffset);
        public Item[] Items
        {
            get
            {
                var reader = new ReaderMiragePrismPrismSetConvert(Base);
                var entries = new Item[reader.Items.Count];
                for(var i = 0; i < entries.Length; i++)
                    entries[i] = new(reader.Items[i]);
                return entries;
            }
        }
        public uint GlamourPrismsHeld => new ReaderMiragePrismPrismSetConvert(Base).GlamourPrismsHeld;
        public uint GlamourPrismsRequired => new ReaderMiragePrismPrismSetConvert(Base).ItemCount;

        public List<int> SlotsFilled => Enumerable.Range(0, Items.Length).Where(x => Items[x].InventoryType != 9999).ToList();
        public int? FirstUnfilledSlot => SlotsFilled.Count == Items.Length ? null : Enumerable.Range(0, Items.Length).FirstOrDefault(i => !SlotsFilled.Contains(i));

        public bool? TryHandOver(int slot)
        {
            if(SlotsFilled.Contains(slot)) return true;

            var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ContextIconMenu", 1);

            if(contextMenu is null || !contextMenu->IsVisible)
            {
                Callback.Fire(Base, true, 13, slot);
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
