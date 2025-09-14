using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.GameHelpers;
public static unsafe class TradeDetectionManager
{
    private static Dictionary<uint, uint> Snapshot;
    private static ulong PartnerCID;
    private static bool Initialized = false;
    private static readonly object LockObj = new();

    private static void Initialize()
    {
        Initialized = true;
        Svc.Condition.ConditionChange += Condition_ConditionChange;
    }

    public delegate void OnTradeStartDelegate(IPlayerCharacter? counterparty);
    private static event OnTradeStartDelegate OnTradeStartInternal;
    public static event OnTradeStartDelegate OnTradeStart
    {
        add
        {
            lock(LockObj)
            {
                if(!Initialized)
                {
                    Initialize();
                }
            }
            OnTradeStartInternal += value;
        }
        remove
        {
            OnTradeStartInternal -= value;
        }
    }

    public delegate void OnTradeEndDelegate(IPlayerCharacter? counterparty, TradeDescriptor? result);
    private static event OnTradeEndDelegate OnTradeEndInternal;
    public static event OnTradeEndDelegate OnTradeEnd
    {
        add
        {
            lock(LockObj)
            {
                if(!Initialized)
                {
                    Initialize();
                }
            }
            OnTradeEndInternal += value;
        }
        remove
        {
            OnTradeEndInternal -= value;
        }
    }

    public static IPlayerCharacter GetTradePartner()
    {
        //Client::Game::InventoryManager_SendTradeRequest
        var id = *(uint*)(((nint)InventoryManager.Instance()) + 8612);
        return Svc.Objects.OfType<IPlayerCharacter>().FirstOrDefault(x => x.EntityId == id);
    }

    private static void Condition_ConditionChange(ConditionFlag flag, bool value)
    {
        if(flag == ConditionFlag.TradeOpen)
        {
            if(value)
            {
                //trade starts
                var tradePartner = GetTradePartner();
                if(tradePartner != null)
                {
                    PartnerCID = tradePartner.Struct()->ContentId;
                }
                OnTradeStartInternal?.Invoke(tradePartner);
                Snapshot = GetInventorySnapshot(ValidInventories);
            }
            else
            {
                //trade ends
                var tradePartner = GetTradePartner();
                if(tradePartner != null)
                {
                    PartnerCID = tradePartner.Struct()->ContentId;
                }
                if(Snapshot != null && PartnerCID != 0)
                {
                    var result = GetTradeResult(PartnerCID, Snapshot, GetInventorySnapshot(ValidInventories));
                    PluginLog.Verbose($"Trade result with {tradePartner}:\n{result}");
                    OnTradeEndInternal?.Invoke(tradePartner, (result.ReceivedItems.Length != 0 || result.ReceivedGil != 0) ? result : null);
                }
                PartnerCID = 0;
                Snapshot = null;
            }
        }
    }

    internal static void Dispose()
    {
        Svc.Condition.ConditionChange -= Condition_ConditionChange;
    }

    public static readonly InventoryType[] ValidInventories = [
        InventoryType.Inventory1,
        InventoryType.Inventory2,
        InventoryType.Inventory3,
        InventoryType.Inventory4,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryMainHand,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryOffHand,
        InventoryType.ArmoryRings,
        InventoryType.ArmoryWaist,
        InventoryType.ArmoryWrist,
        InventoryType.Crystals,
        ];

    public static Dictionary<uint, uint> GetInventorySnapshot(IEnumerable<InventoryType> validInventories)
    {
        var im = InventoryManager.Instance();
        var ret = new Dictionary<uint, uint>
        {
            [1] = (uint)im->GetInventoryItemCount(1)
        };
        foreach(var type in validInventories)
        {
            var inv = im->GetInventoryContainer(type);
            for(var i = 0; i < inv->GetSize(); i++)
            {
                var slot = inv->GetInventorySlot(i);
                var id = slot->GetItemId();
                if(id != 0)
                {
                    if(!ret.ContainsKey(id))
                    {
                        ret[id] = slot->GetQuantity();
                    }
                    else
                    {
                        ret[id] += slot->GetQuantity();
                    }
                }
            }
        }
        return ret;
    }

    public static TradeDescriptor GetTradeResult(ulong cid, Dictionary<uint, uint> invStart, Dictionary<uint, uint> invEnd)
    {
        var gilDiff = (int)invEnd[1] - (int)invStart[1];
        Dictionary<uint, int> diff = [];
        foreach(var x in invStart.Keys.Concat(invEnd.Keys))
        {
            if(x == 1) continue;
            diff[x] = (int)invEnd.SafeSelect(x, 0u) - (int)invStart.SafeSelect(x, 0u);
        }
        return new()
        {
            TradePartnerCID = cid,
            ReceivedGil = gilDiff,
            ReceivedItems = [.. diff.Where(x => x.Value != 0).Select(x => new ItemWithQuantity(x.Key, x.Value))],
        };
    }

    public class TradeDescriptor
    {
        public ulong TradePartnerCID;
        public int ReceivedGil;
        public ItemWithQuantity[] ReceivedItems = [];

        public TradeDescriptor()
        {
        }

        public override string ToString()
        {
            return $"""
            TradePartnerCID: {TradePartnerCID:X16},
            Gil: {ReceivedGil};
            Items: 
            {ReceivedItems?.Select(x => $"    {x}").Print("\n")}
            """;
        }
    }

    public class ItemWithQuantity
    {
        [JsonProperty("ItemID")] public uint ItemID;
        [JsonProperty("Quantity")] public int Quantity;

        public ItemWithQuantity() { }

        public ItemWithQuantity(uint itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{ExcelItemHelper.GetName(ItemID % 1000000, true)}" + (ItemID > 1000000 ? " HQ" : "") + $" x{Quantity}";
        }
    }
}