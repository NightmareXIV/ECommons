using Dalamud.Game.Network;
using Dalamud.Hooking;
using Dalamud.Utility;
using ECommons.LazyDataHelpers;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Network;
using Serilog;
using System;
using System.Runtime.InteropServices;

namespace ECommons.DalamudServices.Legacy;
internal sealed unsafe class GameNetwork : IGameNetwork
{
    private readonly GameNetworkAddressResolver address;
    private readonly Hook<PacketDispatcher.Delegates.OnReceivePacket> processZonePacketDownHook;
    private readonly Hook<ProcessZonePacketUpDelegate> processZonePacketUpHook;

    private readonly HitchDetector hitchDetectorUp;
    private readonly HitchDetector hitchDetectorDown;

    internal unsafe GameNetwork()
    {
        this.hitchDetectorUp = new HitchDetector("GameNetworkUp", 30);
        this.hitchDetectorDown = new HitchDetector("GameNetworkDown", 30);

        this.address = new GameNetworkAddressResolver();
        this.address.Setup(Svc.SigScanner);

        var onReceivePacketAddress = (nint)PacketDispatcher.StaticVirtualTablePointer->OnReceivePacket;

        PluginLog.Debug("===== G A M E N E T W O R K =====");
        PluginLog.Debug($"OnReceivePacket address {Util.DescribeAddress(onReceivePacketAddress)}");
        PluginLog.Debug($"ProcessZonePacketUp address {Util.DescribeAddress(this.address.ProcessZonePacketUp)}");

        this.processZonePacketDownHook = Svc.Hook.HookFromAddress<PacketDispatcher.Delegates.OnReceivePacket>(onReceivePacketAddress, this.ProcessZonePacketDownDetour);
        this.processZonePacketUpHook = Svc.Hook.HookFromAddress<ProcessZonePacketUpDelegate>(this.address.ProcessZonePacketUp, this.ProcessZonePacketUpDetour);

        this.processZonePacketDownHook.Enable();
        this.processZonePacketUpHook.Enable();

        Purgatory.Add(Dispose);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate byte ProcessZonePacketUpDelegate(IntPtr a1, IntPtr dataPtr, IntPtr a3, byte a4);

    /// <inheritdoc/>
    public event IGameNetwork.OnNetworkMessageDelegate? NetworkMessage;

    /// <inheritdoc/>
    private void Dispose()
    {
        PluginLog.Debug($"Disposing GameNetwork");
        this.processZonePacketDownHook.Dispose();
        this.processZonePacketUpHook.Dispose();
    }

    private void ProcessZonePacketDownDetour(PacketDispatcher* dispatcher, uint targetId, IntPtr dataPtr)
    {
        this.hitchDetectorDown.Start();

        // Go back 0x10 to get back to the start of the packet header
        dataPtr -= 0x10;

        foreach(var d in Delegate.EnumerateInvocationList(this.NetworkMessage))
        {
            try
            {
                d.Invoke(
                    dataPtr + 0x20,
                    (ushort)Marshal.ReadInt16(dataPtr, 0x12),
                    0,
                    targetId,
                    NetworkMessageDirection.ZoneDown);
            }
            catch(Exception ex)
            {
                string header;
                try
                {
                    var data = new byte[32];
                    Marshal.Copy(dataPtr, data, 0, 32);
                    header = BitConverter.ToString(data);
                }
                catch(Exception)
                {
                    header = "failed";
                }

                Svc.Log.Error(ex, "Exception on ProcessZonePacketDown hook. Header: " + header);
            }
        }

        this.processZonePacketDownHook.Original(dispatcher, targetId, dataPtr + 0x10);
        this.hitchDetectorDown.Stop();
    }

    private byte ProcessZonePacketUpDetour(IntPtr a1, IntPtr dataPtr, IntPtr a3, byte a4)
    {
        this.hitchDetectorUp.Start();

        try
        {
            // Call events
            // TODO: Implement actor IDs
            this.NetworkMessage?.Invoke(dataPtr + 0x20, (ushort)Marshal.ReadInt16(dataPtr), 0x0, 0x0, NetworkMessageDirection.ZoneUp);
        }
        catch(Exception ex)
        {
            string header;
            try
            {
                var data = new byte[32];
                Marshal.Copy(dataPtr, data, 0, 32);
                header = BitConverter.ToString(data);
            }
            catch(Exception)
            {
                header = "failed";
            }

            Svc.Log.Error(ex, "Exception on ProcessZonePacketUp hook. Header: " + header);
        }

        this.hitchDetectorUp.Stop();

        return this.processZonePacketUpHook.Original(a1, dataPtr, a3, a4);
    }
}
