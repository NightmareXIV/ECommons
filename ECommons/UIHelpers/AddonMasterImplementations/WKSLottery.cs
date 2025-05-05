using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    public unsafe partial class WKSLottery : AddonMasterBase<AtkUnitBase>
    {
        public WKSLottery(nint addon) : base(addon) { }
        public WKSLottery(void* addon) : base(addon) { }

        public AtkComponentButton* WheelLeftButton => Addon->GetButtonNodeById(29);
        public AtkComponentButton* WheelRightButton => Addon->GetButtonNodeById(39);
        public AtkComponentButton* SpinWheelButton => Addon->GetButtonNodeById(64);

        public WheelItems[] LeftWheelItems
        {
            get
            {
                var ret = new List<WheelItems>();
                for(var i = 0; i < 7; i++)
                {
                    var itemId = Addon->AtkValues[89 + i * 7].UInt;
                    if(itemId == 0)
                        continue;

                    var itemAmount = Addon->AtkValues[92 + i * 7].UInt;
                    SeString itemName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[91 + i * 7].String.Value).GetText();
                    string itemNameText = itemName.ToString();

                    var ItemList = new WheelItems(this)
                    {
                        itemId = itemId,
                        itemAmount = itemAmount,
                        itemName = itemNameText
                    };
                    ret.Add(ItemList);
                }
                return [.. ret];
            }
        }

        public WheelItems[] RightWheelItems
        {
            get
            {
                var ret = new List<WheelItems>();
                for(var i = 0; i < 7; i++)
                {
                    var itemId = Addon->AtkValues[138 + i * 7].UInt;
                    if(itemId == 0)
                        continue;

                    var itemAmount = Addon->AtkValues[141 + i * 7].UInt;
                    SeString itemName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[140 + i * 7].String.Value).GetText();
                    string itemNameText = itemName.ToString();

                    var ItemList = new WheelItems(this)
                    {
                        itemId = itemId,
                        itemAmount = itemAmount,
                        itemName = itemNameText
                    };
                    ret.Add(ItemList);
                }
                return [.. ret];
            }
        }

        public class WheelItems(WKSLottery master)
        {
            public uint itemId;
            public required string itemName;
            public uint itemAmount;
        }

        public void SelectWheelLeft()
        {
            var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("WKSLottery", 1);

            Callback.Fire(contextMenu, true, 0, 0);
            Callback.Fire(contextMenu, true, 1, 0);
            PluginLog.Debug($"Selecting Left Wheel");
        }

        /* Not... fully implimented correctly. Need to figure out what's the callback for this one
        public void SelectWheelRight()
        {
            var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("WKSLottery", 1);

            Callback.Fire(contextMenu, true, 0, 0);
            Callback.Fire(contextMenu, true, 1, 1);
            PluginLog.Debug($"Selecting Right Wheel");
        }
        */

        public void ConfirmButton() => ClickButtonIfEnabled(SpinWheelButton);


        public override string AddonDescription => "Steller Missions Lottery Ui";
    }
}